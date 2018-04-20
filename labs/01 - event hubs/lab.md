# LAB01 - Getting Connected

[![N|Solid](https://cldup.com/dTxpPi9lDf.thumb.png)](https://nodesource.com/products/nsolid)
## Create GlobalAzureBootcamp Resource Group

### Step 1 - Log in to Azure Portal.

### Step 2 - Click "Create a resource".

### Step 3 - Type "resource group" in the search box and press Enter.

### Step 4 - Select Resource Group.

### Step 5 - Click the "Create" button.

### Step 6 - Enter the name of the resource group - GlobalAzureBootcamp2018. Select The appropriate subscription. Select "West US 2" for the location.

### Step 7 - Click the "Create" button.

[![N|Solid](https://cldup.com/dTxpPi9lDf.thumb.png)](https://nodesource.com/products/nsolid)
## Configure Event Hubs

### Step 1 - Click "Create a resource"

### Step 2 - Type "Event Hubs" in the search box and press Enter.

### Step 3 - Select "Event Hubs"

### Step 4 - Click the "Create" button.

### Step 5 - Enter the name eh3522p{attendeeNumber} for the name of the hub.

### Step 6 - Select the "Standard" pricing tier, the "GlobalAzureBootcamp2018" resource group, the "West US 2" region and disable "Enable auto-inflate".

### Step 7 - Click the "Create" button.

### Step 8 - Navigate to the event hub namespace by selecting "Resource Groups", selecting the "GlobalAzureBootcamp2018" resource group and clicking the name of the Event Hub namespace you created.

### Step 9 - Select "Event Hubs" under the "Entities" section.

### Step 10 - Click the "+ Event Hub" button

### Step 11 - Enter the name deviceevents as the name. Leave all other settings alone.

### Step 12 - Click the "Create" button.

### Step 13 - Navigate to the newly created hub by selecting the "Event Hubs" tab on the left, using the resource creation alert or other means.

### Step 14 - Select the newly created hub.

### Step 15 - Select "Shared access policies" on the left hand panel.

### Step 16 - Click the "Add" button and provide the name sender and click the "send" checkbox.

### Step 17 - Click the "Create" button.

### Step 18 - Once completed, select the policy. Copy the "Primary Key" value and store it for later use. This will be used to authenticate to the event hub.

### Step 19 - Open the \utilities\sasgen folder and launch the signature generator tool.

### Step 20 - Enter the following values:

Namespace: The name you gave your event hub namespace in step 5.
Hub Name: deviceevents
Publisher: globalazure
Mode: http
Sender Key Name: sender
Sender Key: the value copied in Step 18.
Token TTL: 480

### Step 21 - Click the "Generate" button and copy (and save) the value provided in "Signature".

## Verify Connectivity

### Step 1 - Launch the Restlet Client from Chrome.

### Step 2 - Select the "POST" method, and enter the URL: https://{namecreatedinstep5}.servicebus.windows.net/deviceevents/publishers/globalazure/messages/

### Step 3 - Create an Authorization header and provide the value recorded from Step 21 in the previous section. 

### Step 4 - Create a Content-Type header and set it to application/json.

### Step 5 - Enter {"test":"event"} in the Body textbox.

### Step 6 - Click "Send" and verify that a 201 response was returned.