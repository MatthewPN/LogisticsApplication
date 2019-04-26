# Logistics Application User Manual 
Contributors: Matthew Notter, Chad O’Bryan, Joe Massaro, Darin Lawrence

## This application uses uniform cost search and heuristic algorithms to find the best path between two nodes on the map. You can either add your own nodes to the map or use a pre-drawn map. 
1.	By default it will have a pre-created map available.
2.	To find the best path between two nodes, click Run Logistics. Then, click a starting node (one circle) and an ending node (another circle).  
3.	After an ideal path is found, information is displayed at the bottom of the window.  The length of the path in miles is displayed, followed by the number of nodes expanded in the regular uniform cost search and the heuristic uniform cost search.  

`"Expansions" relates to uniform cost search and "Heuristic Expansions" relates to the heuristic algorithm.`

**NOTE:** you can add extra nodes and connections if desired using the follow procedures below, if you wish to create a new map, there is a file “data.json” which holds all the data that can be deleted or moved.  

## To create your own map to experiment:
1.	On a picture box (labeled picturebox1) you can add nodes through the functions.  
2.	To add a node click “Functions” -> “New Markers”. You can procedure to click the picture box as many times as you want to place nodes.  
3.	To add connections between those nodes,   
  a.	Click “Functions” -> “Edit” -> “Connect Markers”.  
  b.	Click and drag from one node to another to create a connection between them.  
  c.	You must click and release within a node or else it will not create a path, additionally, if it is clicked or released in two different nodes it will make a path to whichever node is mathematically closer.
4.	You must save the data manually, every time you add a node manually it will print to output the all the JSON data it has created.  You must copy from the starting “{“ to the ending “}” and place it next to the executable with the name “data.json” so that it loads it on the next run (This is by default in the `LogisticsApplication-master\LogisticsApplication\bin\Debug` folder of the project).



