#!/bin/bash

# ECS Fargate Deployment Script for FIlms Application
# This script deploys the Docker image to AWS ECS Fargate

set -e  # Exit on error
set -o pipefail  # Exit on pipe failure

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}FIlms ECS Fargate Deployment Script${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""

# Prompt for AWS configuration
echo -e "${YELLOW}=== AWS Configuration ===${NC}"
read -p "Enter AWS region (e.g., us-east-1): " AWS_REGION
read -p "Enter ECS cluster name (e.g., films-cluster): " CLUSTER_NAME
read -p "Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/films:latest): " IMAGE_URI

echo ""
echo -e "${YELLOW}=== Network Configuration ===${NC}"
read -p "Enter VPC ID (e.g., vpc-0abc123def456): " VPC_ID
read -p "Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): " SUBNET_IDS
read -p "Enter Security Group ID (e.g., sg-0abc123def): " SECURITY_GROUP

# Split subnet IDs
IFS=',' read -ra SUBNETS <<< "$SUBNET_IDS"
SUBNET_1="${SUBNETS[0]}"
SUBNET_2="${SUBNETS[1]:-$SUBNET_1}"

echo ""
echo -e "${YELLOW}=== Storage Configuration ===${NC}"
read -p "Do you want to use EFS for persistent storage? (y/n): " USE_EFS

if [ "$USE_EFS" == "y" ] || [ "$USE_EFS" == "Y" ]; then
    read -p "Enter EFS File System ID (e.g., fs-0abc123def): " EFS_FILE_SYSTEM_ID
else
    EFS_FILE_SYSTEM_ID=""
fi

echo ""
echo -e "${YELLOW}=== Load Balancer Configuration ===${NC}"
read -p "Do you need a load balancer for this service? (y/n): " NEED_LB

if [ "$NEED_LB" == "y" ] || [ "$NEED_LB" == "Y" ]; then
    echo -e "${GREEN}Creating Application Load Balancer...${NC}"
    
    # Create ALB
    ALB_NAME="films-alb"
    ALB_ARN=$(aws elbv2 create-load-balancer \
        --name "$ALB_NAME" \
        --subnets "$SUBNET_1" "$SUBNET_2" \
        --security-groups "$SECURITY_GROUP" \
        --scheme internet-facing \
        --type application \
        --ip-address-type ipv4 \
        --region "$AWS_REGION" \
        --query 'LoadBalancers[0].LoadBalancerArn' \
        --output text 2>/dev/null || echo "")
    
    if [ -z "$ALB_ARN" ]; then
        echo -e "${YELLOW}Load balancer may already exist, attempting to retrieve...${NC}"
        ALB_ARN=$(aws elbv2 describe-load-balancers \
            --names "$ALB_NAME" \
            --region "$AWS_REGION" \
            --query 'LoadBalancers[0].LoadBalancerArn' \
            --output text 2>/dev/null || echo "")
    fi
    
    if [ -z "$ALB_ARN" ]; then
        echo -e "${RED}Failed to create or retrieve load balancer${NC}"
        exit 1
    fi
    
    echo -e "${GREEN}Load Balancer ARN: $ALB_ARN${NC}"
    
    # Get VPC ID from subnet if not provided
    if [ -z "$VPC_ID" ]; then
        VPC_ID=$(aws ec2 describe-subnets \
            --subnet-ids "$SUBNET_1" \
            --region "$AWS_REGION" \
            --query 'Subnets[0].VpcId' \
            --output text)
    fi
    
    # Create Target Group with target-type ip (required for Fargate)
    TG_NAME="films-tg"
    TARGET_GROUP_ARN=$(aws elbv2 create-target-group \
        --name "$TG_NAME" \
        --protocol HTTP \
        --port 8080 \
        --vpc-id "$VPC_ID" \
        --target-type ip \
        --health-check-enabled \
        --health-check-protocol HTTP \
        --health-check-path "/HealthCheck.aspx" \
        --health-check-interval-seconds 30 \
        --health-check-timeout-seconds 5 \
        --healthy-threshold-count 2 \
        --unhealthy-threshold-count 3 \
        --region "$AWS_REGION" \
        --query 'TargetGroups[0].TargetGroupArn' \
        --output text 2>/dev/null || echo "")
    
    if [ -z "$TARGET_GROUP_ARN" ]; then
        echo -e "${YELLOW}Target group may already exist, attempting to retrieve...${NC}"
        TARGET_GROUP_ARN=$(aws elbv2 describe-target-groups \
            --names "$TG_NAME" \
            --region "$AWS_REGION" \
            --query 'TargetGroups[0].TargetGroupArn' \
            --output text 2>/dev/null || echo "")
    fi
    
    if [ -z "$TARGET_GROUP_ARN" ]; then
        echo -e "${RED}Failed to create or retrieve target group${NC}"
        exit 1
    fi
    
    echo -e "${GREEN}Target Group ARN: $TARGET_GROUP_ARN${NC}"
    
    # Create Listener
    LISTENER_ARN=$(aws elbv2 create-listener \
        --load-balancer-arn "$ALB_ARN" \
        --protocol HTTP \
        --port 80 \
        --default-actions Type=forward,TargetGroupArn="$TARGET_GROUP_ARN" \
        --region "$AWS_REGION" \
        --query 'Listeners[0].ListenerArn' \
        --output text 2>/dev/null || echo "")
    
    if [ -z "$LISTENER_ARN" ]; then
        echo -e "${YELLOW}Listener may already exist${NC}"
    else
        echo -e "${GREEN}Listener created successfully${NC}"
    fi
    
    # Get ALB DNS name
    ALB_DNS=$(aws elbv2 describe-load-balancers \
        --load-balancer-arns "$ALB_ARN" \
        --region "$AWS_REGION" \
        --query 'LoadBalancers[0].DNSName' \
        --output text)
    
    echo -e "${GREEN}Load Balancer DNS: $ALB_DNS${NC}"
else
    TARGET_GROUP_ARN=""
fi

# Get AWS Account ID
echo ""
echo -e "${GREEN}Retrieving AWS Account ID...${NC}"
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
echo -e "${GREEN}Account ID: $ACCOUNT_ID${NC}"

# Check/Create ECS Cluster
echo ""
echo -e "${GREEN}Checking ECS cluster...${NC}"
CLUSTER_EXISTS=$(aws ecs describe-clusters \
    --clusters "$CLUSTER_NAME" \
    --region "$AWS_REGION" \
    --query 'clusters[0].clusterName' \
    --output text 2>/dev/null || echo "None")

if [ "$CLUSTER_EXISTS" == "None" ] || [ -z "$CLUSTER_EXISTS" ]; then
    echo -e "${YELLOW}Cluster does not exist. Creating...${NC}"
    aws ecs create-cluster --cluster-name "$CLUSTER_NAME" --region "$AWS_REGION"
    echo -e "${GREEN}Cluster created successfully!${NC}"
else
    echo -e "${GREEN}Cluster already exists${NC}"
fi

# Create CloudWatch Log Group
echo ""
echo -e "${GREEN}Creating CloudWatch log group...${NC}"
aws logs create-log-group \
    --log-group-name "/ecs/films-app" \
    --region "$AWS_REGION" 2>/dev/null || echo -e "${YELLOW}Log group may already exist${NC}"

# Prepare task definition
echo ""
echo -e "${GREEN}Preparing task definition...${NC}"
TASK_DEF_FILE="ecs/task-definition.json"
TASK_DEF_TEMP="ecs/task-definition-temp.json"

cp "$TASK_DEF_FILE" "$TASK_DEF_TEMP"

# Replace placeholders
sed -i "s|{{IMAGE_URI}}|$IMAGE_URI|g" "$TASK_DEF_TEMP"
sed -i "s|{{AWS_REGION}}|$AWS_REGION|g" "$TASK_DEF_TEMP"
sed -i "s|{{ACCOUNT_ID}}|$ACCOUNT_ID|g" "$TASK_DEF_TEMP"

# Remove EFS volumes if not using EFS
if [ -z "$EFS_FILE_SYSTEM_ID" ]; then
    echo -e "${YELLOW}Removing EFS configuration from task definition...${NC}"
    # Remove volumes and mountPoints sections
    python3 -c "
import json
import sys

with open('$TASK_DEF_TEMP', 'r') as f:
    task_def = json.load(f)

# Remove volumes
task_def['volumes'] = []

# Remove mountPoints from container
for container in task_def['containerDefinitions']:
    if 'mountPoints' in container:
        del container['mountPoints']

with open('$TASK_DEF_TEMP', 'w') as f:
    json.dump(task_def, f, indent=2)
" 2>/dev/null || {
        # Fallback if Python is not available
        echo -e "${YELLOW}Python not available, keeping EFS placeholders${NC}"
    }
else
    sed -i "s|{{EFS_FILE_SYSTEM_ID}}|$EFS_FILE_SYSTEM_ID|g" "$TASK_DEF_TEMP"
fi

# Register task definition
echo ""
echo -e "${GREEN}Registering task definition...${NC}"
TASK_DEF_ARN=$(aws ecs register-task-definition \
    --cli-input-json file://"$TASK_DEF_TEMP" \
    --region "$AWS_REGION" \
    --query 'taskDefinition.taskDefinitionArn' \
    --output text)

if [ -z "$TASK_DEF_ARN" ]; then
    echo -e "${RED}Failed to register task definition${NC}"
    exit 1
fi

echo -e "${GREEN}Task definition registered: $TASK_DEF_ARN${NC}"

# Prepare service definition
echo ""
echo -e "${GREEN}Preparing service definition...${NC}"
SERVICE_DEF_FILE="ecs/service-definition.json"
SERVICE_DEF_TEMP="ecs/service-definition-temp.json"

cp "$SERVICE_DEF_FILE" "$SERVICE_DEF_TEMP"

# Replace placeholders
sed -i "s|{{CLUSTER_NAME}}|$CLUSTER_NAME|g" "$SERVICE_DEF_TEMP"
sed -i "s|{{SUBNET_1}}|$SUBNET_1|g" "$SERVICE_DEF_TEMP"
sed -i "s|{{SUBNET_2}}|$SUBNET_2|g" "$SERVICE_DEF_TEMP"
sed -i "s|{{SECURITY_GROUP}}|$SECURITY_GROUP|g" "$SERVICE_DEF_TEMP"

# Handle load balancer configuration
if [ -z "$TARGET_GROUP_ARN" ]; then
    echo -e "${YELLOW}Removing load balancer configuration from service definition...${NC}"
    python3 -c "
import json

with open('$SERVICE_DEF_TEMP', 'r') as f:
    service_def = json.load(f)

# Remove loadBalancers and healthCheckGracePeriodSeconds
if 'loadBalancers' in service_def:
    del service_def['loadBalancers']
if 'healthCheckGracePeriodSeconds' in service_def:
    del service_def['healthCheckGracePeriodSeconds']

with open('$SERVICE_DEF_TEMP', 'w') as f:
    json.dump(service_def, f, indent=2)
" 2>/dev/null || {
        echo -e "${YELLOW}Python not available, keeping load balancer placeholders${NC}"
    }
else
    sed -i "s|{{TARGET_GROUP_ARN}}|$TARGET_GROUP_ARN|g" "$SERVICE_DEF_TEMP"
fi

# Check if service exists
SERVICE_NAME="films-service"
SERVICE_EXISTS=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[0].serviceName' \
    --output text 2>/dev/null || echo "None")

if [ "$SERVICE_EXISTS" == "None" ] || [ -z "$SERVICE_EXISTS" ]; then
    # Create new service
    echo ""
    echo -e "${GREEN}Creating ECS service...${NC}"
    aws ecs create-service \
        --cli-input-json file://"$SERVICE_DEF_TEMP" \
        --region "$AWS_REGION"
    
    if [ $? -ne 0 ]; then
        echo -e "${RED}Failed to create service${NC}"
        exit 1
    fi
    
    echo -e "${GREEN}Service created successfully!${NC}"
else
    # Update existing service
    echo ""
    echo -e "${GREEN}Updating existing ECS service...${NC}"
    aws ecs update-service \
        --cluster "$CLUSTER_NAME" \
        --service "$SERVICE_NAME" \
        --task-definition "$TASK_DEF_ARN" \
        --desired-count 2 \
        --region "$AWS_REGION"
    
    if [ $? -ne 0 ]; then
        echo -e "${RED}Failed to update service${NC}"
        exit 1
    fi
    
    echo -e "${GREEN}Service updated successfully!${NC}"
fi

# Wait for service stability
echo ""
echo -e "${GREEN}Waiting for service to become stable...${NC}"
echo -e "${YELLOW}This may take several minutes...${NC}"

aws ecs wait services-stable \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION"

if [ $? -eq 0 ]; then
    echo -e "${GREEN}Service is stable!${NC}"
else
    echo -e "${YELLOW}Service stability check timed out or failed${NC}"
fi

# Verify deployment
echo ""
echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}Deployment Summary${NC}"
echo -e "${GREEN}========================================${NC}"

SERVICE_INFO=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[0]')

RUNNING_COUNT=$(echo "$SERVICE_INFO" | grep -o '"runningCount": [0-9]*' | grep -o '[0-9]*')
DESIRED_COUNT=$(echo "$SERVICE_INFO" | grep -o '"desiredCount": [0-9]*' | grep -o '[0-9]*')

echo -e "${GREEN}Cluster: $CLUSTER_NAME${NC}"
echo -e "${GREEN}Service: $SERVICE_NAME${NC}"
echo -e "${GREEN}Task Definition: $TASK_DEF_ARN${NC}"
echo -e "${GREEN}Running Tasks: $RUNNING_COUNT / $DESIRED_COUNT${NC}"
echo -e "${GREEN}CloudWatch Logs: /ecs/films-app${NC}"

if [ -n "$ALB_DNS" ]; then
    echo ""
    echo -e "${GREEN}Application URL: http://$ALB_DNS${NC}"
    echo -e "${GREEN}Health Check: http://$ALB_DNS/HealthCheck.aspx${NC}"
fi

echo ""
echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}Deployment Completed Successfully!${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""

# Cleanup temp files
rm -f "$TASK_DEF_TEMP" "$SERVICE_DEF_TEMP"

echo -e "${YELLOW}Troubleshooting Tips:${NC}"
echo "1. View logs: aws logs tail /ecs/films-app --follow --region $AWS_REGION"
echo "2. Check tasks: aws ecs list-tasks --cluster $CLUSTER_NAME --region $AWS_REGION"
echo "3. Describe service: aws ecs describe-services --cluster $CLUSTER_NAME --services $SERVICE_NAME --region $AWS_REGION"
echo ""
