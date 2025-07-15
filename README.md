## MCP Azure Infrastructure Assistant

**MCP Azure Infrastructure Assistant** is a powerful server built with Model Context Protocol (MCP) that allows you to **analyze, manage, and automate Azure infrastructure using natural language**, especially via Claude. It seamlessly integrates SDK-based operations with Infrastructure-as-Code provisioning using Terraform.

## Key Features

-  **Built-in Infrastructure Analyzer** – Evaluates deployed resources against best practices and provides clear insights (✅ Good | ⚠️ Warnings | 🟥 Issues)
-  **Infrastructure-as-Code with Terraform** – Provision VMs and networks with dynamic Terraform templates generated at runtime
-  **Azure Resource Management** – List, create, and analyze Azure Resource Groups, VMs, Networks, Storage, Cosmos DB, and more
-  **Embedded Best Practices Engine** – Uses internal knowledge base to compare infrastructure health and compliance
-  **Secure Authentication** – Leverages environment variables to authenticate securely with Azure (supports managed identity or service principal)
-  **Fully Containerized** – Ready-to-run with Docker and available on Docker Hub
-  **Natural Language Integration with Claude** – Enables intelligent interactions through `claude_desktop_config.json` for AI-assisted workflows

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

### Subscriptions
- **List Subscriptions** – Displays all accessible Azure subscriptions.

### Storage
- **List Storage Accounts** – Lists all storage accounts across the subscription.
- **Analyze Storage Accounts** – Reviews encryption, network restrictions, access settings, and tagging against best practices.

### Virtual Machines (Compute)
- **Deploy Virtual Machine** – Uses Terraform to deploy a Linux VM with network, IP, and disk configuration.
- **List Virtual Machines** – Lists all VMs in the subscription.
- **List VMs by Resource Group** – Lists VMs inside a specific resource group.
- **Analyze VM Best Practices** – Evaluates VM configuration for encryption, patching, diagnostics, network rules, and more.

### Networking
- **List Virtual Networks** – Lists all Virtual Networks (VNets) in the subscription.
- **List Subnets by VNET** – Lists subnets inside a specific VNet.
- **List NSGs by Resource Group** – Lists Network Security Groups within a specific resource group.
- **Analyze Virtual Networks** – Examines peering, address spaces, subnets, NSG/routing associations, and DDoS settings.

### Key Vault
- **Analyze Key Vaults** – Reviews soft delete, purge protection, RBAC, public access, and deployment options.

### App Services
- **Analyze App Services** – Analyzes web app configurations including logging, VNet integration, TLS, deployment slots, and more.

### Containers
- **Analyze Container Instances** – Reviews CPU/memory usage, image details, networking, and restart policies of ACI deployments.

### Log Analytics & Monitoring
- **List Log Analytics Workspaces** – Shows all Log Analytics workspaces available.
- **Configure Diagnostic Settings** – Enables diagnostics to a Log Analytics workspace for a given resource.


### Knowledge Base
- **List Best Practices** – Retrieves best practices for a specific Azure resource type (e.g., `vm`, `vnet`, `storage`, `keyvault`, `appservices`, `containers`, `security`, `cost-optimization`, `performance`, `governance`, `monitoring`).

### Infrastructure Analysis
- **Analyze Resource Group** – Performs an intelligent review of infrastructure components (VMs, VNETs, NSGs, Tags, etc.) within a resource group and compares them against embedded best practices.
  - Outputs a categorized report:
    - ✅ Best practices met
    - ⚠️ Recommendations
    - 🟥 Critical issues

---

>  All tools are integrated with Model Context Protocol and optimized for AI comprehension.

 *More tools and features are on the way!*
