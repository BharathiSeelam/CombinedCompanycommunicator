# CombinedCompanycommunicator
Add the below code to the manifest.json
"composeExtensions": [  
  {  
    "botId": "e2c34a9a-282f-4725-8f87-0590f9103fd8",  
    "canUpdateConfiguration": true,  
    "commands": [
        {
          "id": "Corp Comms",
          "title": "Search",
		  "context": [ "compose", "commandBox" ],
          "description": "Seacrh By Title",
          "initialRun": true,
		  "type": "query",  
          "parameters": [
            {
              "name": "searchText",
              "title": "Recent",
              "description": "Seacrh By Title",
			  "inputType": "text"  
            }
          ]
        }
      ] 
  }  
]   
