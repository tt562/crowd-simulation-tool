# Crowd Simulation Tool

The following instructions will assume that you have access to Unity.

Once the project has been loaded to Unity, there are a few functionalities that you should be made aware of:

### Customising the number of each size of group spawned:

  Select the Spawn Area game object in the Unity Hierarchy
  
  Go to the Unity Inspector and see the section Group Distribution
  
  If you want to spawn 3 groups of size 1, set the number next to Element 0 as 3. If you want to spawn 2 groups of size 4, set the number next to Element 3 as 2
  

### Customising the probability a group chooses a certain destination type:

  Select the NPCGroup1 game object in the Unity Hierarchy
  
  Go to the Unity Inspector and see the variables Outdoors Prob, Indoors Prob and Stationary Prob
  
  These three float values must add to 1 to work correctly. The higher the probability set, the more chance a group will have of selecting that destination type
  

### Changing a group's destination:

  If it is desired to select a particular destination point for a group, simply find the destination point game object to be set. This will be in the Unity Hierarchy
  
  Then select the group game object that the destination needs to be changed for
  
  Drag and drop the destination game object into the group's destination public variable in the Unity Inspector
  
  The group will now walk towards this destination instead
  
### Changing a group's formation

  A group's formation can be changed during run-time
  
  Simply click on the group game object in the Unity Hierarchy
  
  Then in the Unity Inspector, go to the Shape public variable and select the desired formation from the drop-down menu
  
  The formation should update automatically
  
  
  
  
If any issues are encountered with running the Crowd Simulation Tool or if there are any particular features that want to be showcased, email tt562@bath.ac.uk
