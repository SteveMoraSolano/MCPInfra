## MCP Azure Infrastructure Assistant

**MCP Azure Infrastructure Assistant** is a powerful server built with Model Context Protocol (MCP) that allows you to **analyze, manage, and automate Azure infrastructure using natural language**, especially via Claude. It seamlessly integrates SDK-based operations with Infrastructure-as-Code provisioning using Terraform.

## Key Features

-  **Secure Authentication** – Leverages environment variables to authenticate securely with Azure (supports managed identity or service principal)
-  **Azure Resource Management** – List, create, and analyze Azure Resource Groups, VMs, Networks, Storage, Cosmos DB, and more
-  **Natural Language Integration with Claude** – Enables intelligent interactions through `claude_desktop_config.json` for AI-assisted workflows
-  **Infrastructure-as-Code with Terraform** – Provision VMs and networks with dynamic Terraform templates generated at runtime
-  **Built-in Infrastructure Analyzer** – Evaluates deployed resources against best practices and provides clear insights (✅ Good | ⚠️ Warnings | 🟥 Issues)
-  **Embedded Best Practices Engine** – Uses internal knowledge base (no external API calls) to compare infrastructure health and compliance
-  **Fully Containerized** – Ready-to-run with Docker and available on Docker Hub

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
- **Create Resource Group** – Creates a new resource group.
- **List Resources in Resource Group** – Lists all resources within a specific resource group.

### App Configuration
- **List App Configuration Stores** – Retrieves all App Configuration instances in the subscription.

### Subscriptions
- **List Subscriptions** – Displays all accessible Azure subscriptions.

### Cosmos DB
- **List Cosmos DB Accounts** – Lists all Cosmos DB accounts in the subscription.
- **List Cosmos DB Accounts by Resource Group** – Lists Cosmos DB accounts in a specific resource group.

### Log Analytics & Monitoring
- **List Log Analytics Workspaces** – Shows all Log Analytics workspaces available.
- **Configure Diagnostic Settings** – Enables diagnostics to a Log Analytics workspace for a given resource.

###  Storage
- **List Storage Accounts** – Lists all storage accounts across the subscription.

###  Virtual Machines (Compute)
- **Deploy Virtual Machine** – Uses Terraform to deploy a VM with customizable inputs.
- **List Virtual Machines** – Lists all VMs in the subscription.
- **List VMs by Resource Group** – Lists VMs inside a specific resource group.
- **Analyze VM Best Practices** – Evaluates VM configuration against security, availability, and cost optimization best practices.

### Networking
- **List Virtual Networks** – Lists all Virtual Networks (VNETs) in the subscription.
- **List Subnets by VNET** – Lists subnets inside a specific Virtual Network.
- **List NSGs by Resource Group** – Lists Network Security Groups within a specific resource group.

### Knowledge Base
- **List Best Practices** – Retrieves best practices for a specific Azure resource type (e.g., `vm`, `storage`, `keyvault`, `cost-optimization`, etc.). Supports aliases like `costo`, `vm`, `nsg`, etc.

### Infrastructure Analysis
- **Analyze Resource Group** – Performs an intelligent review of infrastructure components (VMs, VNETs, NSGs, Tags, etc.) within a resource group and compares them against embedded best practices.
  - Outputs a categorized report:
    - ✅ Best practices met
    - ⚠️ Recommendations
    - 🟥 Critical issues

---

>  All tools are integrated with Model Context Protocol and optimized for AI comprehension.

 *More tools and features are on the way!*
