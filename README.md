# CombinedCompanycommunicator
Add the below code to the manifest.json

"composeExtensions": [  
  {  
    "botId": "e2c34a9a-282f-4725-8f87-0590f9103fd8",  
    "canUpdateConfiguration": true,  
    "commands": [
        {
          "id": "Corp Comms",
          "title": "Recent",
		  "context": [ "compose", "commandBox" ],
          "description": "Seacrh for Recent",
          "initialRun": true,
          "parameters": [
            {
              "name": "searchText",
              "title": "Recent",
              "description": ""
            }
          ]
        }
      ] 
  }  
]  
