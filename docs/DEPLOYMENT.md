# FIlms Application - AWS ECS Fargate Deployment Guide

## Table of Contents
1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Architecture](#architecture)
4. [Local Development](#local-development)
5. [AWS ECS Fargate Setup](#aws-ecs-fargate-setup)
6. [Deployment Process](#deployment-process)
7. [Configuration Management](#configuration-management)
8. [Monitoring and Logging](#monitoring-and-logging)
9. [Troubleshooting](#troubleshooting)
10. [Security Considerations](#security-considerations)
11. [Scaling and Performance](#scaling-and-performance)

---

## Overview

This guide provides comprehensive instructions for deploying the **FIlms** ASP.NET Web Application to AWS ECS Fargate. The application is a .NET 8.0 web application with Entity Framework Core, containerized using Docker and deployed to AWS ECS Fargate for scalable, serverless container orchestration.

### Technology Stack
- **Framework**: .NET 8.0 (ASP.NET Web Application)
- **Database**: SQL Server with Entity Framework Core
- **Container Runtime**: Docker
- **Orchestration**: AWS ECS Fargate
- **Load Balancer**: AWS Application Load Balancer (ALB)
- **Logging**: AWS CloudWatch Logs
- **Storage**: AWS EFS (optional for persistent data)

---

## Prerequisites

### Required Tools
1. **Docker Desktop** (v20.10 or later)
   - Download: https://www.docker.com/products/docker-desktop
   - Verify: `docker --version`

2. **AWS CLI** (v2.x)
   - Download: https://aws.amazon.com/cli/
   - Verify: `aws --version`
   - Configure: `aws configure`

3. **.NET SDK 8.0** (for local development)
   - Download: https://dotnet.microsoft.com/download/dotnet/8.0
   - Verify: `dotnet --version`

4. **Git** (for version control)
   - Download: https://git-scm.com/
   - Verify: `git --version`

### AWS Account Requirements
- Active AWS account with appropriate permissions
- IAM user with the following permissions:
  - ECS Full Access
  - ECR Full Access
  - EC2 (for VPC, subnets, security groups)
  - CloudWatch Logs
  - IAM (for creating roles)
  - Elastic Load Balancing (for ALB)
  - EFS (optional, for persistent storage)

### AWS Resources Needed
- **VPC** with at least 2 subnets in different availability zones
- **Security Group** allowing inbound traffic on port 8080 (application) and 80 (ALB)
- **IAM Roles**:
  - `ecsTaskExecutionRole` - For ECS to pull images and write logs
  - `ecsTaskRole` - For application to access AWS services (optional)

---

## Architecture

### Container Architecture
```
┌─────────────────────────────────────────────────────────────┐
│                     AWS ECS Fargate                         │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐  │
│  │              Application Load Balancer                │  │
│  │                  (Port 80 HTTP)                       │  │
│  └────────────────────┬─────────────────────────────────┘  │
│                       │                                     │
│  ┌────────────────────┴─────────────────────────────────┐  │
│  │              Target Group (IP mode)                   │  │
│  │              Health Check: /HealthCheck.aspx          │  │
│  └────────────────────┬─────────────────────────────────┘  │
│                       │                                     │
│  ┌────────────────────┴─────────────────────────────────┐  │
│  │                 ECS Service                           │  │
│  │           (Desired Count: 2 tasks)                    │  │
│  │                                                       │  │
│  │  ┌─────────────────┐      ┌─────────────────┐       │  │
│  │  │   Task 1        │      │   Task 2        │       │  │
│  │  │  ┌───────────┐  │      │  ┌───────────┐  │       │  │
│  │  │  │ FIlms App │  │      │  │ FIlms App │  │       │  │
│  │  │  │ Port 8080 │  │      │  │ Port 8080 │  │       │  │
│  │  │  └───────────┘  │      │  └───────────┘  │       │  │
│  │  └─────────────────┘      └─────────────────┘       │  │
│  └───────────────────────────────────────────────────────┘  │
│                                                             │
│  ┌───────────────────────────────────────────────────────┐  │
│  │              CloudWatch Logs                          │  │
│  │           Log Group: /ecs/films-app                   │  │
│  └───────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

### Multi-Stage Docker Build
```
Stage 1: Builder (mcr.microsoft.com/dotnet/sdk:8.0)
  ├── Copy project files
  ├── Restore NuGet packages
  ├── Build application (Release)
  └── Publish application

Stage 2: Runtime (mcr.microsoft.com/dotnet/aspnet:8.0)
  ├── Copy published artifacts
  ├── Create non-root user
  ├── Set environment variables
  └── Run application on port 8080
```

---

## Local Development

### Building the Docker Image Locally

1. **Navigate to project directory**:
   ```bash
   cd /path/to/Newcontainercheck
   ```

2. **Build the Docker image**:
   ```bash
   docker build -t films-app:latest -f Dockerfile .
   ```

3. **Run the container locally**:
   ```bash
   docker run -d \
     -p 8080:8080 \
     -e ASPNETCORE_ENVIRONMENT=Development \
     -e ConnectionStrings__DefaultConnection="Server=host.docker.internal;Database=films;User Id=sa;Password=YourPassword123;TrustServerCertificate=True;" \
     --name films-app \
     films-app:latest
   ```

4. **Access the application**:
   - Application: http://localhost:8080
   - Health Check: http://localhost:8080/HealthCheck.aspx

5. **View logs**:
   ```bash
   docker logs -f films-app
   ```

6. **Stop and remove container**:
   ```bash
   docker stop films-app
   docker rm films-app
   ```

### Using Docker Compose

1. **Start the application**:
   ```bash
   docker-compose up -d
   ```

2. **View logs**:
   ```bash
   docker-compose logs -f
   ```

3. **Stop the application**:
   ```bash
   docker-compose down
   ```

---

## AWS ECS Fargate Setup

### Step 1: Create IAM Roles

#### ECS Task Execution Role
```bash
aws iam create-role \
  --role-name ecsTaskExecutionRole \
  --assume-role-policy-document '{
    "Version": "2012-10-17",
    "Statement": [{
      "Effect": "Allow",
      "Principal": {"Service": "ecs-tasks.amazonaws.com"},
      "Action": "sts:AssumeRole"
    }]
  }'

aws iam attach-role-policy \
  --role-name ecsTaskExecutionRole \
  --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy
```

#### ECS Task Role (Optional - for application AWS access)
```bash
aws iam create-role \
  --role-name ecsTaskRole \
  --assume-role-policy-document '{
    "Version": "2012-10-17",
    "Statement": [{
      "Effect": "Allow",
      "Principal": {"Service": "ecs-tasks.amazonaws.com"},
      "Action": "sts:AssumeRole"
    }]
  }'
```

### Step 2: Create VPC and Network Resources

If you don't have a VPC, create one:

```bash
# Create VPC
VPC_ID=$(aws ec2 create-vpc \
  --cidr-block 10.0.0.0/16 \
  --query 'Vpc.VpcId' \
  --output text)

# Create Internet Gateway
IGW_ID=$(aws ec2 create-internet-gateway \
  --query 'InternetGateway.InternetGatewayId' \
  --output text)

aws ec2 attach-internet-gateway \
  --vpc-id $VPC_ID \
  --internet-gateway-id $IGW_ID

# Create Subnets (2 in different AZs)
SUBNET_1=$(aws ec2 create-subnet \
  --vpc-id $VPC_ID \
  --cidr-block 10.0.1.0/24 \
  --availability-zone us-east-1a \
  --query 'Subnet.SubnetId' \
  --output text)

SUBNET_2=$(aws ec2 create-subnet \
  --vpc-id $VPC_ID \
  --cidr-block 10.0.2.0/24 \
  --availability-zone us-east-1b \
  --query 'Subnet.SubnetId' \
  --output text)

# Create Route Table
ROUTE_TABLE=$(aws ec2 create-route-table \
  --vpc-id $VPC_ID \
  --query 'RouteTable.RouteTableId' \
  --output text)

aws ec2 create-route \
  --route-table-id $ROUTE_TABLE \
  --destination-cidr-block 0.0.0.0/0 \
  --gateway-id $IGW_ID

aws ec2 associate-route-table \
  --route-table-id $ROUTE_TABLE \
  --subnet-id $SUBNET_1

aws ec2 associate-route-table \
  --route-table-id $ROUTE_TABLE \
  --subnet-id $SUBNET_2

# Create Security Group
SG_ID=$(aws ec2 create-security-group \
  --group-name films-sg \
  --description "Security group for FIlms application" \
  --vpc-id $VPC_ID \
  --query 'GroupId' \
  --output text)

# Allow inbound traffic
aws ec2 authorize-security-group-ingress \
  --group-id $SG_ID \
  --protocol tcp \
  --port 8080 \
  --cidr 0.0.0.0/0

aws ec2 authorize-security-group-ingress \
  --group-id $SG_ID \
  --protocol tcp \
  --port 80 \
  --cidr 0.0.0.0/0
```

### Step 3: Create CloudWatch Log Group

```bash
aws logs create-log-group \
  --log-group-name /ecs/films-app \
  --region us-east-1
```

### Step 4: Create ECS Cluster

```bash
aws ecs create-cluster \
  --cluster-name films-cluster \
  --region us-east-1
```

### Step 5: (Optional) Create EFS File System

For persistent storage across container restarts:

```bash
# Create EFS File System
EFS_ID=$(aws efs create-file-system \
  --performance-mode generalPurpose \
  --throughput-mode bursting \
  --encrypted \
  --tags Key=Name,Value=films-efs \
  --query 'FileSystemId' \
  --output text)

# Create Mount Targets in each subnet
aws efs create-mount-target \
  --file-system-id $EFS_ID \
  --subnet-id $SUBNET_1 \
  --security-groups $SG_ID

aws efs create-mount-target \
  --file-system-id $EFS_ID \
  --subnet-id $SUBNET_2 \
  --security-groups $SG_ID
```

---

## Deployment Process

### Method 1: Automated Deployment (Recommended)

#### Step 1: Build and Push Docker Image

**Linux/macOS**:
```bash
cd /path/to/Newcontainercheck
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```

**Windows**:
```cmd
cd C:\path\to\Newcontainercheck
scripts\build-push.bat
```

The script will:
1. Prompt for registry selection (AWS ECR or Docker Hub)
2. Prompt for registry credentials
3. Build the Docker image
4. Push to the selected registry
5. Display the image URI for deployment

#### Step 2: Deploy to ECS Fargate

**Linux/macOS**:
```bash
chmod +x scripts/deploy-image.sh
./scripts/deploy-image.sh
```

**Windows**:
```cmd
scripts\deploy-image.bat
```

The script will:
1. Prompt for AWS region and cluster name
2. Prompt for network configuration (VPC, subnets, security group)
3. Prompt for Docker image URI
4. Optionally create Application Load Balancer
5. Register task definition
6. Create or update ECS service
7. Wait for service stability
8. Display deployment summary

### Method 2: Manual Deployment

#### Step 1: Build and Push to ECR

```bash
# Authenticate with ECR
aws ecr get-login-password --region us-east-1 | \
  docker login --username AWS --password-stdin \
  123456789.dkr.ecr.us-east-1.amazonaws.com

# Create ECR repository
aws ecr create-repository \
  --repository-name films \
  --region us-east-1

# Build image
docker build -t films:latest .

# Tag image
docker tag films:latest \
  123456789.dkr.ecr.us-east-1.amazonaws.com/films:latest

# Push image
docker push 123456789.dkr.ecr.us-east-1.amazonaws.com/films:latest
```

#### Step 2: Update Task Definition

Edit `ecs/task-definition.json` and replace placeholders:
- `{{IMAGE_URI}}` - Your ECR image URI
- `{{AWS_REGION}}` - Your AWS region
- `{{ACCOUNT_ID}}` - Your AWS account ID
- `{{EFS_FILE_SYSTEM_ID}}` - Your EFS ID (if using EFS)

#### Step 3: Register Task Definition

```bash
aws ecs register-task-definition \
  --cli-input-json file://ecs/task-definition.json \
  --region us-east-1
```

#### Step 4: Create/Update Service

Edit `ecs/service-definition.json` and replace placeholders:
- `{{CLUSTER_NAME}}` - Your cluster name
- `{{SUBNET_1}}`, `{{SUBNET_2}}` - Your subnet IDs
- `{{SECURITY_GROUP}}` - Your security group ID
- `{{TARGET_GROUP_ARN}}` - Your target group ARN (if using ALB)

```bash
# Create service
aws ecs create-service \
  --cli-input-json file://ecs/service-definition.json \
  --region us-east-1

# Or update existing service
aws ecs update-service \
  --cluster films-cluster \
  --service films-service \
  --task-definition films-task \
  --desired-count 2 \
  --region us-east-1
```

---

## Configuration Management

### Environment Variables

The application uses the following environment variables:

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core environment | Production |
| `ASPNETCORE_URLS` | Application listening URLs | http://+:8080 |
| `DOTNET_RUNNING_IN_CONTAINER` | Indicates container runtime | true |
| `DataFilePath` | Path for data files | /app/data/files |
| `LogFilePath` | Path for log files | /app/logs |
| `TempFilePath` | Path for temporary files | /app/temp |
| `TZ` | Timezone | UTC |

### Database Connection Strings

**Option 1: Environment Variables** (for non-sensitive environments)
```json
{
  "name": "ConnectionStrings__DefaultConnection",
  "value": "Server=myserver.database.windows.net;Database=films;User Id=admin;Password=MyPassword123;"
}
```

**Option 2: AWS Secrets Manager** (recommended for production)
```json
{
  "name": "ConnectionStrings__DefaultConnection",
  "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789:secret:films/db-connection"
}
```

To create a secret:
```bash
aws secretsmanager create-secret \
  --name films/db-connection \
  --secret-string "Server=myserver.database.windows.net;Database=films;User Id=admin;Password=MyPassword123;TrustServerCertificate=True;" \
  --region us-east-1
```

### Application Settings

The application uses `Web.config` for configuration. Key settings:

- **Connection Strings**: Database connections
- **App Settings**: File paths and application-specific settings
- **Authentication**: Forms authentication configuration
- **Session State**: In-process session state

For containerized deployments, these can be overridden using environment variables.

---

## Monitoring and Logging

### CloudWatch Logs

View application logs:
```bash
# Tail logs in real-time
aws logs tail /ecs/films-app --follow --region us-east-1

# View logs for specific time range
aws logs filter-log-events \
  --log-group-name /ecs/films-app \
  --start-time $(date -d '1 hour ago' +%s)000 \
  --region us-east-1
```

### ECS Service Metrics

Monitor service health:
```bash
# Describe service
aws ecs describe-services \
  --cluster films-cluster \
  --services films-service \
  --region us-east-1

# List running tasks
aws ecs list-tasks \
  --cluster films-cluster \
  --service-name films-service \
  --region us-east-1

# Describe task
aws ecs describe-tasks \
  --cluster films-cluster \
  --tasks <task-id> \
  --region us-east-1
```

### Health Checks

The application exposes a health check endpoint:
- **URL**: `/HealthCheck.aspx`
- **Method**: GET
- **Success Response**: HTTP 200 with JSON `{"status":"healthy","timestamp":"..."}`
- **Failure Response**: HTTP 503 with JSON `{"status":"unhealthy","error":"...","timestamp":"..."}`

ALB health check configuration:
- **Path**: `/HealthCheck.aspx`
- **Interval**: 30 seconds
- **Timeout**: 5 seconds
- **Healthy Threshold**: 2
- **Unhealthy Threshold**: 3

---

## Troubleshooting

### Common Issues

#### 1. Task Fails to Start

**Symptoms**: Tasks start and immediately stop

**Possible Causes**:
- Invalid Docker image URI
- Insufficient IAM permissions
- Invalid CPU/memory configuration
- Application crashes on startup

**Solutions**:
```bash
# Check task stopped reason
aws ecs describe-tasks \
  --cluster films-cluster \
  --tasks <task-id> \
  --region us-east-1 \
  --query 'tasks[0].stoppedReason'

# Check CloudWatch logs
aws logs tail /ecs/films-app --follow --region us-east-1

# Verify task definition
aws ecs describe-task-definition \
  --task-definition films-task \
  --region us-east-1
```

#### 2. Cannot Pull Docker Image

**Symptoms**: "CannotPullContainerError"

**Possible Causes**:
- ECR authentication failure
- Invalid image URI
- Missing executionRoleArn permissions

**Solutions**:
```bash
# Verify ECR repository exists
aws ecr describe-repositories \
  --repository-names films \
  --region us-east-1

# Check execution role permissions
aws iam get-role-policy \
  --role-name ecsTaskExecutionRole \
  --policy-name AmazonECSTaskExecutionRolePolicy

# Manually test image pull
docker pull 123456789.dkr.ecr.us-east-1.amazonaws.com/films:latest
```

#### 3. Service Unhealthy

**Symptoms**: Tasks running but health checks failing

**Possible Causes**:
- Application not listening on correct port
- Health check endpoint not responding
- Security group blocking traffic
- Application startup taking too long

**Solutions**:
```bash
# Check target group health
aws elbv2 describe-target-health \
  --target-group-arn <target-group-arn> \
  --region us-east-1

# Verify security group rules
aws ec2 describe-security-groups \
  --group-ids <security-group-id> \
  --region us-east-1

# Test health endpoint directly
curl http://<task-ip>:8080/HealthCheck.aspx

# Increase health check grace period
aws ecs update-service \
  --cluster films-cluster \
  --service films-service \
  --health-check-grace-period-seconds 300 \
  --region us-east-1
```

#### 4. Database Connection Failures

**Symptoms**: Application logs show database connection errors

**Possible Causes**:
- Invalid connection string
- Database not accessible from ECS tasks
- Security group blocking database port
- Secrets Manager permissions missing

**Solutions**:
```bash
# Verify connection string in Secrets Manager
aws secretsmanager get-secret-value \
  --secret-id films/db-connection \
  --region us-east-1

# Check task role permissions for Secrets Manager
aws iam list-attached-role-policies \
  --role-name ecsTaskExecutionRole

# Test database connectivity from task
aws ecs execute-command \
  --cluster films-cluster \
  --task <task-id> \
  --container films-app \
  --interactive \
  --command "/bin/bash"
```

#### 5. Out of Memory Errors

**Symptoms**: Tasks killed with "OutOfMemoryError"

**Solutions**:
- Increase memory allocation in task definition
- Valid Fargate CPU/Memory combinations:
  - CPU: 512 → Memory: 1024, 2048
  - CPU: 1024 → Memory: 2048, 3072, 4096, 5120, 6144, 7168, 8192
  - CPU: 2048 → Memory: 4096-16384 (increments of 1024)

```json
{
  "cpu": "1024",
  "memory": "2048"
}
```

### Debugging Commands

```bash
# View service events
aws ecs describe-services \
  --cluster films-cluster \
  --services films-service \
  --region us-east-1 \
  --query 'services[0].events[0:10]'

# Get task details
aws ecs describe-tasks \
  --cluster films-cluster \
  --tasks <task-id> \
  --region us-east-1

# Execute command in running container
aws ecs execute-command \
  --cluster films-cluster \
  --task <task-id> \
  --container films-app \
  --interactive \
  --command "/bin/bash"

# View CloudWatch metrics
aws cloudwatch get-metric-statistics \
  --namespace AWS/ECS \
  --metric-name CPUUtilization \
  --dimensions Name=ServiceName,Value=films-service Name=ClusterName,Value=films-cluster \
  --start-time $(date -u -d '1 hour ago' +%Y-%m-%dT%H:%M:%S) \
  --end-time $(date -u +%Y-%m-%dT%H:%M:%S) \
  --period 300 \
  --statistics Average \
  --region us-east-1
```

---

## Security Considerations

### Container Security

1. **Non-Root User**: The Dockerfile creates and uses a non-root user (`appuser`) for running the application

2. **Read-Only Root Filesystem**: Consider adding read-only root filesystem in task definition:
   ```json
   {
     "readonlyRootFilesystem": true
   }
   ```

3. **Security Scanning**: Scan Docker images for vulnerabilities:
   ```bash
   aws ecr start-image-scan \
     --repository-name films \
     --image-id imageTag=latest \
     --region us-east-1
   ```

### Network Security

1. **Security Groups**: Restrict inbound traffic to only necessary ports
   - Application: Port 8080 (from ALB only)
   - ALB: Port 80/443 (from internet)

2. **Private Subnets**: Deploy tasks in private subnets with NAT Gateway for outbound access

3. **VPC Endpoints**: Use VPC endpoints for AWS services (ECR, Secrets Manager, CloudWatch)

### Secrets Management

1. **Never hardcode secrets** in Dockerfile or task definition

2. **Use AWS Secrets Manager** for sensitive data:
   ```bash
   aws secretsmanager create-secret \
     --name films/db-password \
     --secret-string "MySecurePassword123" \
     --region us-east-1
   ```

3. **Reference secrets in task definition**:
   ```json
   {
     "secrets": [
       {
         "name": "DB_PASSWORD",
         "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789:secret:films/db-password"
       }
     ]
   }
   ```

### IAM Best Practices

1. **Principle of Least Privilege**: Grant only necessary permissions

2. **Separate Roles**:
   - **Task Execution Role**: For ECS to pull images and write logs
   - **Task Role**: For application to access AWS services

3. **Enable CloudTrail**: Audit all API calls

---

## Scaling and Performance

### Auto Scaling

#### Target Tracking Scaling

Scale based on CPU utilization:
```bash
aws application-autoscaling register-scalable-target \
  --service-namespace ecs \
  --scalable-dimension ecs:service:DesiredCount \
  --resource-id service/films-cluster/films-service \
  --min-capacity 2 \
  --max-capacity 10 \
  --region us-east-1

aws application-autoscaling put-scaling-policy \
  --service-namespace ecs \
  --scalable-dimension ecs:service:DesiredCount \
  --resource-id service/films-cluster/films-service \
  --policy-name films-cpu-scaling \
  --policy-type TargetTrackingScaling \
  --target-tracking-scaling-policy-configuration '{
    "TargetValue": 70.0,
    "PredefinedMetricSpecification": {
      "PredefinedMetricType": "ECSServiceAverageCPUUtilization"
    },
    "ScaleInCooldown": 300,
    "ScaleOutCooldown": 60
  }' \
  --region us-east-1
```

#### Step Scaling

Scale based on custom metrics:
```bash
aws application-autoscaling put-scaling-policy \
  --service-namespace ecs \
  --scalable-dimension ecs:service:DesiredCount \
  --resource-id service/films-cluster/films-service \
  --policy-name films-step-scaling \
  --policy-type StepScaling \
  --step-scaling-policy-configuration '{
    "AdjustmentType": "PercentChangeInCapacity",
    "StepAdjustments": [
      {
        "MetricIntervalLowerBound": 0,
        "MetricIntervalUpperBound": 10,
        "ScalingAdjustment": 10
      },
      {
        "MetricIntervalLowerBound": 10,
        "ScalingAdjustment": 30
      }
    ],
    "Cooldown": 60
  }' \
  --region us-east-1
```

### Performance Optimization

#### .NET Runtime Optimizations

1. **ReadyToRun Images**: Use ReadyToRun compilation for faster startup:
   ```bash
   dotnet publish -c Release -r linux-x64 --self-contained false /p:PublishReadyToRun=true
   ```

2. **Tiered Compilation**: Enable tiered compilation (enabled by default in .NET 8.0)

3. **Garbage Collection**: Configure GC for containerized environments:
   ```json
   {
     "environment": [
       {
         "name": "DOTNET_gcServer",
         "value": "1"
       },
       {
         "name": "DOTNET_GCHeapCount",
         "value": "2"
       }
     ]
   }
   ```

#### Container Optimizations

1. **Multi-Stage Builds**: Already implemented in Dockerfile

2. **Layer Caching**: Order Dockerfile commands from least to most frequently changing

3. **Image Size**: Use minimal base images (aspnet vs sdk)

#### Database Optimizations

1. **Connection Pooling**: Enable in connection string:
   ```
   Server=myserver;Database=films;User Id=admin;Password=pass;Max Pool Size=100;Min Pool Size=10;
   ```

2. **Read Replicas**: Use read replicas for read-heavy workloads

3. **Caching**: Implement caching layer (Redis, ElastiCache)

### Blue/Green Deployments

Use ECS deployment circuit breaker for safe deployments:
```json
{
  "deploymentConfiguration": {
    "deploymentCircuitBreaker": {
      "enable": true,
      "rollback": true
    }
  }
}
```

Or use CodeDeploy for blue/green deployments:
```bash
aws deploy create-deployment \
  --application-name films-app \
  --deployment-group-name films-dg \
  --deployment-config-name CodeDeployDefault.ECSAllAtOnce \
  --region us-east-1
```

---

## Additional Resources

### AWS Documentation
- [ECS Fargate Documentation](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/AWS_Fargate.html)
- [ECS Task Definitions](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/task_definitions.html)
- [ECS Service Auto Scaling](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/service-auto-scaling.html)

### .NET Documentation
- [.NET 8.0 Documentation](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [ASP.NET Core in Containers](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

### Docker Documentation
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [Multi-Stage Builds](https://docs.docker.com/develop/develop-images/multistage-build/)

---

## Support and Maintenance

### Monitoring Checklist
- [ ] CloudWatch alarms configured for CPU/Memory
- [ ] Log aggregation and analysis setup
- [ ] Health check endpoints monitored
- [ ] Database connection pool monitored
- [ ] Application performance metrics tracked

### Regular Maintenance Tasks
- [ ] Update base Docker images monthly
- [ ] Review and rotate secrets quarterly
- [ ] Audit IAM permissions quarterly
- [ ] Review and optimize costs monthly
- [ ] Test disaster recovery procedures quarterly
- [ ] Update .NET runtime and dependencies as needed

### Cost Optimization
- [ ] Use Fargate Spot for non-critical workloads
- [ ] Implement auto-scaling to match demand
- [ ] Use appropriate task sizes (CPU/Memory)
- [ ] Enable CloudWatch Logs retention policies
- [ ] Review and delete unused ECR images

---

## Conclusion

This deployment guide provides comprehensive instructions for deploying the FIlms ASP.NET application to AWS ECS Fargate. Follow the steps carefully, and refer to the troubleshooting section for common issues. For production deployments, ensure all security best practices are implemented and monitoring is properly configured.

For questions or issues, refer to the AWS documentation or contact your DevOps team.

**Happy Deploying! 🚀**
