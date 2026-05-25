@echo off
setlocal enabledelayedexpansion

REM ECS Fargate Deployment Script for FIlms Application (Windows)
REM This script deploys the Docker image to AWS ECS Fargate

echo ========================================
echo FIlms ECS Fargate Deployment Script
echo ========================================
echo.

REM Prompt for AWS configuration
echo === AWS Configuration ===
set /p AWS_REGION="Enter AWS region (e.g., us-east-1): "
set /p CLUSTER_NAME="Enter ECS cluster name (e.g., films-cluster): "
set /p IMAGE_URI="Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/films:latest): "

echo.
echo === Network Configuration ===
set /p VPC_ID="Enter VPC ID (e.g., vpc-0abc123def456): "
set /p SUBNET_IDS="Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): "
set /p SECURITY_GROUP="Enter Security Group ID (e.g., sg-0abc123def): "

REM Split subnet IDs
for /f "tokens=1,2 delims=," %%a in ("!SUBNET_IDS!") do (
    set SUBNET_1=%%a
    set SUBNET_2=%%b
)
if "!SUBNET_2!"=="" set SUBNET_2=!SUBNET_1!

echo.
echo === Storage Configuration ===
set /p USE_EFS="Do you want to use EFS for persistent storage? (y/n): "

if /i "!USE_EFS!"=="y" (
    set /p EFS_FILE_SYSTEM_ID="Enter EFS File System ID (e.g., fs-0abc123def): "
) else (
    set EFS_FILE_SYSTEM_ID=
)

echo.
echo === Load Balancer Configuration ===
set /p NEED_LB="Do you need a load balancer for this service? (y/n): "

if /i "!NEED_LB!"=="y" (
    echo Creating Application Load Balancer...
    
    REM Create ALB
    set ALB_NAME=films-alb
    for /f "delims=" %%i in ('aws elbv2 create-load-balancer --name !ALB_NAME! --subnets !SUBNET_1! !SUBNET_2! --security-groups !SECURITY_GROUP! --scheme internet-facing --type application --ip-address-type ipv4 --region !AWS_REGION! --query "LoadBalancers[0].LoadBalancerArn" --output text 2^>nul') do set ALB_ARN=%%i
    
    if "!ALB_ARN!"=="" (
        echo Load balancer may already exist, attempting to retrieve...
        for /f "delims=" %%i in ('aws elbv2 describe-load-balancers --names !ALB_NAME! --region !AWS_REGION! --query "LoadBalancers[0].LoadBalancerArn" --output text 2^>nul') do set ALB_ARN=%%i
    )
    
    if "!ALB_ARN!"=="" (
        echo Failed to create or retrieve load balancer
        exit /b 1
    )
    
    echo Load Balancer ARN: !ALB_ARN!
    
    REM Get VPC ID from subnet if not provided
    if "!VPC_ID!"=="" (
        for /f "delims=" %%i in ('aws ec2 describe-subnets --subnet-ids !SUBNET_1! --region !AWS_REGION! --query "Subnets[0].VpcId" --output text') do set VPC_ID=%%i
    )
    
    REM Create Target Group with target-type ip
    set TG_NAME=films-tg
    for /f "delims=" %%i in ('aws elbv2 create-target-group --name !TG_NAME! --protocol HTTP --port 8080 --vpc-id !VPC_ID! --target-type ip --health-check-enabled --health-check-protocol HTTP --health-check-path "/HealthCheck.aspx" --health-check-interval-seconds 30 --health-check-timeout-seconds 5 --healthy-threshold-count 2 --unhealthy-threshold-count 3 --region !AWS_REGION! --query "TargetGroups[0].TargetGroupArn" --output text 2^>nul') do set TARGET_GROUP_ARN=%%i
    
    if "!TARGET_GROUP_ARN!"=="" (
        echo Target group may already exist, attempting to retrieve...
        for /f "delims=" %%i in ('aws elbv2 describe-target-groups --names !TG_NAME! --region !AWS_REGION! --query "TargetGroups[0].TargetGroupArn" --output text 2^>nul') do set TARGET_GROUP_ARN=%%i
    )
    
    if "!TARGET_GROUP_ARN!"=="" (
        echo Failed to create or retrieve target group
        exit /b 1
    )
    
    echo Target Group ARN: !TARGET_GROUP_ARN!
    
    REM Create Listener
    for /f "delims=" %%i in ('aws elbv2 create-listener --load-balancer-arn !ALB_ARN! --protocol HTTP --port 80 --default-actions Type=forward,TargetGroupArn=!TARGET_GROUP_ARN! --region !AWS_REGION! --query "Listeners[0].ListenerArn" --output text 2^>nul') do set LISTENER_ARN=%%i
    
    if "!LISTENER_ARN!"=="" (
        echo Listener may already exist
    ) else (
        echo Listener created successfully
    )
    
    REM Get ALB DNS name
    for /f "delims=" %%i in ('aws elbv2 describe-load-balancers --load-balancer-arns !ALB_ARN! --region !AWS_REGION! --query "LoadBalancers[0].DNSName" --output text') do set ALB_DNS=%%i
    
    echo Load Balancer DNS: !ALB_DNS!
) else (
    set TARGET_GROUP_ARN=
)

REM Get AWS Account ID
echo.
echo Retrieving AWS Account ID...
for /f "delims=" %%i in ('aws sts get-caller-identity --query Account --output text') do set ACCOUNT_ID=%%i
echo Account ID: !ACCOUNT_ID!

REM Check/Create ECS Cluster
echo.
echo Checking ECS cluster...
for /f "delims=" %%i in ('aws ecs describe-clusters --clusters !CLUSTER_NAME! --region !AWS_REGION! --query "clusters[0].clusterName" --output text 2^>nul') do set CLUSTER_EXISTS=%%i

if "!CLUSTER_EXISTS!"=="None" (
    echo Cluster does not exist. Creating...
    aws ecs create-cluster --cluster-name !CLUSTER_NAME! --region !AWS_REGION!
    if !ERRORLEVEL! neq 0 (
        echo Failed to create cluster
        exit /b 1
    )
    echo Cluster created successfully!
) else (
    echo Cluster already exists
)

REM Create CloudWatch Log Group
echo.
echo Creating CloudWatch log group...
aws logs create-log-group --log-group-name "/ecs/films-app" --region !AWS_REGION! 2>nul
if !ERRORLEVEL! neq 0 (
    echo Log group may already exist
)

REM Prepare task definition
echo.
echo Preparing task definition...
set TASK_DEF_FILE=ecs\task-definition.json
set TASK_DEF_TEMP=ecs\task-definition-temp.json

copy /Y "!TASK_DEF_FILE!" "!TASK_DEF_TEMP!" >nul

REM Replace placeholders using PowerShell
powershell -Command "(Get-Content '!TASK_DEF_TEMP!') -replace '{{IMAGE_URI}}', '!IMAGE_URI!' | Set-Content '!TASK_DEF_TEMP!'"
powershell -Command "(Get-Content '!TASK_DEF_TEMP!') -replace '{{AWS_REGION}}', '!AWS_REGION!' | Set-Content '!TASK_DEF_TEMP!'"
powershell -Command "(Get-Content '!TASK_DEF_TEMP!') -replace '{{ACCOUNT_ID}}', '!ACCOUNT_ID!' | Set-Content '!TASK_DEF_TEMP!'"

REM Remove EFS volumes if not using EFS
if "!EFS_FILE_SYSTEM_ID!"=="" (
    echo Removing EFS configuration from task definition...
    powershell -Command "$json = Get-Content '!TASK_DEF_TEMP!' | ConvertFrom-Json; $json.volumes = @(); foreach($c in $json.containerDefinitions) { $c.PSObject.Properties.Remove('mountPoints') }; $json | ConvertTo-Json -Depth 10 | Set-Content '!TASK_DEF_TEMP!'"
) else (
    powershell -Command "(Get-Content '!TASK_DEF_TEMP!') -replace '{{EFS_FILE_SYSTEM_ID}}', '!EFS_FILE_SYSTEM_ID!' | Set-Content '!TASK_DEF_TEMP!'"
)

REM Register task definition
echo.
echo Registering task definition...
for /f "delims=" %%i in ('aws ecs register-task-definition --cli-input-json file://!TASK_DEF_TEMP! --region !AWS_REGION! --query "taskDefinition.taskDefinitionArn" --output text') do set TASK_DEF_ARN=%%i

if "!TASK_DEF_ARN!"=="" (
    echo Failed to register task definition
    exit /b 1
)

echo Task definition registered: !TASK_DEF_ARN!

REM Prepare service definition
echo.
echo Preparing service definition...
set SERVICE_DEF_FILE=ecs\service-definition.json
set SERVICE_DEF_TEMP=ecs\service-definition-temp.json

copy /Y "!SERVICE_DEF_FILE!" "!SERVICE_DEF_TEMP!" >nul

REM Replace placeholders
powershell -Command "(Get-Content '!SERVICE_DEF_TEMP!') -replace '{{CLUSTER_NAME}}', '!CLUSTER_NAME!' | Set-Content '!SERVICE_DEF_TEMP!'"
powershell -Command "(Get-Content '!SERVICE_DEF_TEMP!') -replace '{{SUBNET_1}}', '!SUBNET_1!' | Set-Content '!SERVICE_DEF_TEMP!'"
powershell -Command "(Get-Content '!SERVICE_DEF_TEMP!') -replace '{{SUBNET_2}}', '!SUBNET_2!' | Set-Content '!SERVICE_DEF_TEMP!'"
powershell -Command "(Get-Content '!SERVICE_DEF_TEMP!') -replace '{{SECURITY_GROUP}}', '!SECURITY_GROUP!' | Set-Content '!SERVICE_DEF_TEMP!'"

REM Handle load balancer configuration
if "!TARGET_GROUP_ARN!"=="" (
    echo Removing load balancer configuration from service definition...
    powershell -Command "$json = Get-Content '!SERVICE_DEF_TEMP!' | ConvertFrom-Json; $json.PSObject.Properties.Remove('loadBalancers'); $json.PSObject.Properties.Remove('healthCheckGracePeriodSeconds'); $json | ConvertTo-Json -Depth 10 | Set-Content '!SERVICE_DEF_TEMP!'"
) else (
    powershell -Command "(Get-Content '!SERVICE_DEF_TEMP!') -replace '{{TARGET_GROUP_ARN}}', '!TARGET_GROUP_ARN!' | Set-Content '!SERVICE_DEF_TEMP!'"
)

REM Check if service exists
set SERVICE_NAME=films-service
for /f "delims=" %%i in ('aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION! --query "services[0].serviceName" --output text 2^>nul') do set SERVICE_EXISTS=%%i

if "!SERVICE_EXISTS!"=="None" (
    REM Create new service
    echo.
    echo Creating ECS service...
    aws ecs create-service --cli-input-json file://!SERVICE_DEF_TEMP! --region !AWS_REGION!
    
    if !ERRORLEVEL! neq 0 (
        echo Failed to create service
        exit /b 1
    )
    
    echo Service created successfully!
) else (
    REM Update existing service
    echo.
    echo Updating existing ECS service...
    aws ecs update-service --cluster !CLUSTER_NAME! --service !SERVICE_NAME! --task-definition !TASK_DEF_ARN! --desired-count 2 --region !AWS_REGION!
    
    if !ERRORLEVEL! neq 0 (
        echo Failed to update service
        exit /b 1
    )
    
    echo Service updated successfully!
)

REM Wait for service stability
echo.
echo Waiting for service to become stable...
echo This may take several minutes...

aws ecs wait services-stable --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION!

if !ERRORLEVEL! equ 0 (
    echo Service is stable!
) else (
    echo Service stability check timed out or failed
)

REM Verify deployment
echo.
echo ========================================
echo Deployment Summary
echo ========================================

for /f "delims=" %%i in ('aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION! --query "services[0].runningCount" --output text') do set RUNNING_COUNT=%%i
for /f "delims=" %%i in ('aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION! --query "services[0].desiredCount" --output text') do set DESIRED_COUNT=%%i

echo Cluster: !CLUSTER_NAME!
echo Service: !SERVICE_NAME!
echo Task Definition: !TASK_DEF_ARN!
echo Running Tasks: !RUNNING_COUNT! / !DESIRED_COUNT!
echo CloudWatch Logs: /ecs/films-app

if not "!ALB_DNS!"=="" (
    echo.
    echo Application URL: http://!ALB_DNS!
    echo Health Check: http://!ALB_DNS!/HealthCheck.aspx
)

echo.
echo ========================================
echo Deployment Completed Successfully!
echo ========================================
echo.

REM Cleanup temp files
del /F /Q "!TASK_DEF_TEMP!" 2>nul
del /F /Q "!SERVICE_DEF_TEMP!" 2>nul

echo Troubleshooting Tips:
echo 1. View logs: aws logs tail /ecs/films-app --follow --region !AWS_REGION!
echo 2. Check tasks: aws ecs list-tasks --cluster !CLUSTER_NAME! --region !AWS_REGION!
echo 3. Describe service: aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION!
echo.

endlocal
