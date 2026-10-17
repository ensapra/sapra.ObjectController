<div align="center">

  <!-- Website -->
  <a href="https://ensapra.com">
    <img src="https://img.shields.io/badge/Website-Visit-1999?style=for-the-badge&logo=googlechrome&logoColor=white" alt="Ensapra Website" />
  </a>

  <!-- Discord -->
  <a href="https://ensapra.com/links/discord">
    <img src="https://img.shields.io/badge/Discord-Join_Server-5865F2?style=for-the-badge&logo=discord&logoColor=white" alt="Join Ensapra Discord" />
  </a>

  <!-- Patreon -->
  <a href="https://ensapra.com/links/patreon">
    <img src="https://img.shields.io/badge/Patreon-Support-FF424D?style=for-the-badge&logo=patreon&logoColor=white" alt="Support Ensapra on Patreon" />
  </a>

  <a href="https://ensapra.com/links/object-controller-asset-store">
    <img src="https://img.shields.io/badge/Asset_Store-Get_for_free!-dba248?style=for-the-badge&logo=unity&logoColor=white" alt="Support Ensapra on Patreon" />
  </a>
  
</div>

# Object Controller
This package contains the basic structure to build an object controller, following a certain workflow. 
It allows the user to create different modules that can be structured so that components inside can be shown, hidden, and overall enabled and disabled 

It contains a samples folder, showing an example of component structure. 

The benefits of using this is allowing for a better component organisation inside a character controller, improving the modularity and flexibility. 

Not only that, but the structure allows for the creation of any type of object controller. 

## Before
As soon as the character controller gets complex and you want to keep modularity on it, components start to overflow the inspector, making it hard to easily find what you are looking for.

![image](https://user-images.githubusercontent.com/61149758/167370149-96e4708e-fda2-43a8-9011-d34de4dfb997.png)

## After
By using this workflow, components are nicely grouped, without losing the control over them.

![image](https://user-images.githubusercontent.com/61149758/167370830-dfe5df6a-1fa1-4d32-b1be-3f4800c9e589.png)

## Implementation
An example of implementation can be found on the Samples Folder:

1 - Create a "controller" deriving from AbstractCControl:
From this script we will manage the diferent modules, time of execution and other generic variables. That's the monobehaviour that will be added to the gameObject. It's the root of everything.

2 - Create "parent routines" deriving from AbstractRoutine:
From this script we will create the functions to be used on all of those routines. It must be System.Serializable, and abstract. That's a leaf.

3 - Create a "module" deriving from AbstractModule of type of the parent routine:
From this script you will manage all the routines, the order of execution by using the list onlyEnabled, and setting a method to be called from the controller. That's a branch.

4 - Create a "child routine" deriving from the previous parent routine: 
From this script you will make the implementation of each action that you might need. Those will be the ants, and it will contain the actual work.

5 - Finally, create a custom property drawer script deriving from ModuleDrawer, for your "module"
This script doesn't require any code in it, and it will allow the module to be rendered

All of the mentioned scripts must contain [System.Serializable].

## References
Mini-Components by Sascha Graeff
  https://gitlab.com/13pixels/tools/Mini-Components/-/tree/master
  Allows to create a containr for components and Scriptable Objects
