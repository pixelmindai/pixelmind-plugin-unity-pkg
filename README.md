# Pixelmind SDK

Unity package that allows you to generate infinite game assets using AI.

## Unity Versions Support

- \>= 2020.x.x

## Install

Package can be used standalone or optionally together with a Pusher websockets package. 
If installed the Pusher package will use websockets to listen for any changes in the 
Asset Generation Process on Runtime and make updates accordingly, 
which in return should improve performance for your games on Runtime.

Simplest way to install the package and it's optional Pusher dependency is to
open `Packages/manifest.json` file of your project with your favourite editor
and add the following in your `dependencies` (make sure to respect JSON commas):

```json
{
 "dependencies": {
  "com.pusher.pusherwebsocketunity": "https://github.com/pusher/pusher-websocket-unity.git#upm",
  "ai.330.pixelmindsdk": "https://github.com/pixelmindai/pixelmind-plugin-unity-package.git"
 }
}
```

or if you don't plan on using Pusher, you can just add the `ai.330.pixelmindsdk` package.

Alternatively you can go to your Unity Project, to `Window > Package Manager` and install the packages using the
`Add package from git URL...` option. 

If using Pusher make sure to use this URL (uses the UPM branch):

`https://github.com/pusher/pusher-websocket-unity.git#upm` 

and for Pixelmind SDK the default one will do the trick:

`https://github.com/pixelmindai/pixelmind-plugin-unity-package.git`

For known issues after installation check the section [below](#known-issues).

## Getting Started

### Samples

After installing the Pixelmind SDK you can go to `Window > Package Manager` and stitch to `Packages: In Project`
tab to locate the package. On the Pixelmind SDK package page there are samples that can be imported in your 
Project. Samples contain some assets and a sample scene to get you started.

After importing the samples load the above mentioned sample scene inside your project which should be located in
`Assets/Samples/Pixelmind SDK/x.x.x/Scenes` folder.

### How to use

#### Pusher 

On the sample scene you loaded there is game object named `Pusher`. If you don't plan on using Pusher you can delete it in order
to avoid any future `The referenced script (PusherManager) on this Behaviour is missing!` warnings.
If you plan on using Pusher on Runtime, add your Pixelmind's API `secret` Key in the designated field and that is it
for that object.

#### Editor

##### Sprites

Next, you will notice 3 game objects for Character, Weapon and Environment. Also there is a
disabled Cube object. You can interact with each of those objects in a similar fashion while in the Editor.

1. Select the Character object for example. 
2. Locate the `Pixelmind Imaginarium` component.
3. Add your Pixelmind's `public` API key in the designated field first.
4. You can leave the `Assign to sprite renderer` option ticked to assign your newly generated sprite to the current object.
5. Click the `Get Generators` button.
6. Fill the required fields (usually `prompt`) marked with an asterisk (`*`), and update the remaining fields per your preference if needed.
7. Click the `Generate` Button.
8. In a few seconds your sprite renderer will be replaced with a new sprite you just created and a folder located in `Assets/Pixelmind SDK Assets` will now hold your newly created sprite and texture.

##### Materials

1. Following a similar course of action as for the sprites above, you can also enable the cube object in the scene.
2. The cube object has a Mesh Renderer and a sample Material assigned.
3. Add the Api key as you would normally.
4. You'll notice that the object has an option `Assign to Material` ticked. Leave it as it is.
5. Following the same set of instructions as for the sprite, you can generate a texture that will now replace a material of the 3D object like the sample cube.

#### Runtime

To be able to generate assets on Runtime you just need to follow these simple steps:

1. Select the game object with the attached component of `Pixelmind Imaginarium`.
2. Add your Pixelmind's `public` API key in the designated field 
3. Click on the 'Enable GUI' button
4. After you run the game a GUI will appear on top of your Game view.
5. Use the GUI in the same manner as you would in the editor (`Get Generators > Enter Prompt > Generate`).

### Known Issues

If you are using an optional Pusher package, after installation on 2021.x.x versions you might get an error saying:

`Assembly 'Packages/com.pusher.pusherwebsocketunity/Packages/PusherClient.2.1.0/lib/net472/PusherClient.dll' will not be loaded due to errors:
PusherClient references strong named Newtonsoft.Json Assembly references: 12.0.0.0 Found in project: 13.0.0.0.`

To resolve the issue go to `Edit > Project Settings > Player > Other Settings > Configuration > Assembly Version Validation` and disable Version Validation.





