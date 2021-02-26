# CombinedCompanycommunicator
Add the below code to the manifest.json

"composeExtensions": [  
  {  
    "botId": "",  
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
