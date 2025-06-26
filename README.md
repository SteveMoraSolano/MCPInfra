## MCP Azure Infrastructure Assistant

Is a powerful Model Context Protocol (MCP) server tool designed to help you **interact with and automate Azure infrastructure** 
using natural language through Claude. It supports both Azure SDK operations and Infrastructure-as-Code deployments with Terraform.

## Features

- 🔐 Secure authentication with Azure using environment variables
- ☁️ List and manage Azure Resource Groups
- ⚙️ Deploy Azure resources using Terraform (e.g. Resource Groups, VMs)
- 🤖 Easy integration with Claude via `claude_desktop_config.json`
- 🐳 Fully containerized with Docker and published to Docker Hub

---

##  Prerequisites

### On the Host Machine
- [Docker](https://www.docker.com/) installed and running
- [Claude App](https://claude.ai/download) installed

### On Azure
- An Azure Subscription
- A registered **App Registration (client)** with:
  - `Client ID`
  - `Tenant ID`
  - `Client Secret`
- Assigned the **Contributor** role on the subscription

## How to use with Claude
- Edit your `claude_desktop_config.json` file and add the following configuration:
```json
  {
  "mcpServers": {
    "infrawizard": {
      "command": "docker",
      "args": [
        "run",
        "--rm",
        "-i",
        "stevemora/infrawizard:latest"
      ],
      "env": {
        "AZ_TENANT_ID": "your-tenant-id",
        "AZ_CLIENT_ID": "your-client-id",
        "AZ_CLIENT_SECRET": "your-client-secret",
        "AZ_SUBSCRIPTION_ID": "your-subscription-id"
      }
    }
  }
```
Once configured, Claude will automatically pull and execute the container when prompted with tasks related to Azure infrastructure.

## Available MCP Tools
- ListResourceGroups: Lists all resource groups in your Azure subscription.
- DeployVirtualMachine:	Deploys a VM using Terraform.
- More tools coming soon!
