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
    "azmcp": {
      "command": "docker",
      "args": [
        "run",
        "--rm",
        "-i",
        "stevemora/azmcp:latest"
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
### Resource Groups
- **List Resource Groups** – Retrieves all resource groups in the current Azure subscription.
- **Create Resource Group** – Creates a new resource group with user-defined parameters.
- **List Resources in Resource Group** – Lists all resources within a specific resource group.

### App Configuration
- **List App Configuration Stores** – Retrieves all App Configuration instances in the subscription.

### Subscriptions
- **List Subscriptions** – Displays all accessible Azure subscriptions.

### Cosmos DB
- **List Cosmos DB Accounts** – Lists all Cosmos DB accounts in the subscription.

### Log Analytics
- **List Log Analytics Workspaces** – Shows all Log Analytics workspaces available.

### Storage
- **List Storage Accounts** – Lists all storage accounts across the subscription.

### Virtual Machines *(WIP)*
- **Deploy Virtual Machine** – Uses Terraform to deploy a VM with customizable inputs.

---

🔧 *More tools and features are on the way!*
