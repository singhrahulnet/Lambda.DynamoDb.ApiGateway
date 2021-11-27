# Lambda.DynamoDB.ApiGateway
The project has been created using AWS serverless stack. It consists of - 
* Two Lambdas named `StartMining` and `GetMiningStatus`
* The Lambdas are behind an API Gateway (POST and GET)
* A DynamoDB table to store and retrieve data
* AWS CloudFormation SAM model used for Infra as Code
* Unit and Integration tests


## Setup

Download and install `.NET Core 3.1` and IDE of your choice.

Install `Amazon.Lambda.Tools` Global Tools if not already installed.
```
    dotnet tool install -g Amazon.Lambda.Tools
```

If already installed check if new version is available.
```
    dotnet tool update -g Amazon.Lambda.Tools
```

## Running Unit Tests
```
    cd test/ILV.Api.Tests
    dotnet test --filter Unit
```

## Running Integration Tests
Note that integration tests will take a bit longer to run as some tests are written to simulate up to 90 seconds of delay before making an Api call. The tests create a temporary DynamoDB table as well and delete it after execution.

Make sure that an AWS account is _configured_ in your IDE and then run the following command - 

```
    cd test/ILV.Api.Tests
    dotnet test --filter Integration
```

## Deploying the Stack
The AWS settings are defined in `aws-lambda-tools-defaults.json` file. Review it before runnning the command given below.

```
    cd src/ILV.Api
    dotnet lambda deploy-serverless
```
The command will create necessary resources for the app and an API should be ready to use in the API Gateway Service.

## TODOs
Couldn't get the CloudFormation to output the Api endpoint yet. It would be neat to test the endpoints using curl right there in the IDE rather than having to look for the URL in the AWS Console.
