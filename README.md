# Publish / Subscribe pattern using .Net Core and Azure Event Grid Topics
This is to explain how to implement publish / subscribe pattern using Azure Event Grid Topics

## Azure Event Grid Topics
Azure Event Grid allows you to build event-driven applications.
It has [built-in support for events coming from Azure services](https://docs.microsoft.com/en-us/azure/event-grid/overview#event-sources), like storage blobs and resource groups. Event Grid also has support for your own events, using custom topics.

Azure Event Grid uses a pay-per-event pricing model, so you only pay for what you use. The first 100,000 operations per month are free. Operations are defined as event ingress, subscription delivery attempts, management calls, and filtering by subject suffix. 

## Key concepts
* **Events** - What happened.
* **Event sources** - Where the event took place.
* **Topics** - The endpoint where publishers send events.
* **Event subscriptions** - The endpoint or built-in mechanism to route events, sometimes to more than one handler. Subscriptions are also used by handlers to intelligently filter incoming events.
* **Event handlers** - The app or service reacting to the event.

## Security
https://docs.microsoft.com/en-us/azure/event-grid/security-authentication
https://docs.microsoft.com/en-us/azure/event-grid/secure-webhook-delivery

## Prepare the infrastructure
We use PowerShell 7.0 and az module.

### Create the Event Grid Topic
Let's connect to azure first using az PowerShell module:
``` powershell
Import-Module az
Connect-AzAccount
```
We create the resource group:
``` powershell
$resourceGroup = "rg-cace-dev-test-01"
$location = "canadacentral"
New-AzResourceGroup -Name $resourceGroup -Location $location
```

Then we add the Event Grid Topic:
``` powershell
$eventGridTopicName = "my-eventgrid-topic-01"
New-AzEventGridTopic -ResourceGroupName $resourceGroup -Name $eventGridTopicName -Location $location
```

### Add storage queue as a subscriber of the event grid topic
For having more visibility when publishing an event to the event grid topic.
///TODO

### Configure Azure AD application
Create a new Azure AD application:
``` powershell
$adApplication = New-AzADApplication -DisplayName "my-eventgrid-app-01" `
    -IdentifierUris "https://localhost:5003"
```

Adding a new service principal for the Azure AD application which has a Contributor role scoped only to our Event Grid Topic:
``` powershell
$subscriptionId = (Get-AzContext).Subscription.Id
$servicePrincipal = New-AzADServicePrincipal `
    -ApplicationId $adApplication.ApplicationId `
    -Scope "/subscriptions/$subscriptionId/resourceGroups/$resourceGroup/providers/Microsoft.EventGrid/topics/$eventGridTopicName" `
    -Role Contributor
```

Getting the azure role definition:
``` powershell
Get-AzRoleDefinition | Where-Object {$_.Name -like "EventGrid*"} | FT Name, IsCustom, Id
$role = Get-AzRoleDefinition "EventGrid Data Sender"
```

Assign the **EventGrid Contributor** role to service principal to the event grid topic:
``` powershell
New-AzRoleAssignment -ApplicationId $servicePrincipal.ApplicationId `
    -RoleDefinitionName  $role.Name `
    -ResourceName $eventGridTopicName `
    -ResourceType "Microsoft.EventGrid/topics" `
    -ResourceGroupName $resourceGroup
```

Create Client Id & Client Secret for the Azure AD Application.

## Sample asp .net core application 
### Publishing events to Event Grid Topic
Add package references for the following packages:
* Azure.Identity
* Azure.Messaging.EventGrid

Update your appsettings.json file by adding these key/values :
``` json
"ApplicationCredential": {
    "TenantId": "your tenant id",
    "ClientId": "client id (azure ad application id)",
    "ClientSecret": "client secret",
    "EventGridEndpoint": "your eventgrid topci endpoint: https://{your event grid topic name}.{region}-1.eventgrid.azure.net/api/events"
},
```

Send a POST request to this endpoint to publish an event:
```
POST https://localhost:5003/EventPublisher
Content-Type: application/json
{
    {
        "title": "something new happened! again!",
        "amount": 20.99
    }
}
```

### Function App for Event Grid Topic Subscription
* Deploy function app code
* Register function app as a subscription of Event Grid Topic