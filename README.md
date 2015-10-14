# Unity-unet-authoritative-networking

## Goal

This projet aim to provide a base code for a fully authoritative movement in unity3D.
This project isn't an as-is solution. It's usable for demo and educationnal purposes.

I've also made a blog post series about [unity networking and lag compensation techniques](http://www.atrakeur.com/blog/art22-mouvement_autoritatif_en_reseau_partie_1_le_principe) on my own website (In french, sorry).

The code is provided AS-IS without any warranty of any sort. 

Please feel free to use, comment, ask any question or improve the code provided. If you ever use it in any of your project, feel free to improve and publish it back for everyone's sake. I'll happily merge PR as long as they work and keep the same goal.

## How to run

 * Download the project and open the Scenes/Demo scene. 
 * Compile it to a standalone Win/Mac/Linux player. You can also download a compiled version from github.
 * Run at least two instances of the unity game.
 * On one instance, start it as a Server (LAN Host)
 * On others instances, start them as a Client (LAN Client)
 * On Clients, click on spawn to spawn a player and move around with WASD.

## How it run

The server see an instant, snappy and exact version of the world without any latency compensation.
On the moving client, a client side prediction is calculated to predict server behavior and move the player as soon as the player press keys. This predicted position is then reconciliated with server's one later.

On the observing clients, an interpolation algorithm is used to smooth other's clients movement and make it look natural. An extra delay of 100ms is added to ensure that the client has at least 2 states to interpolate between. 

There is no extrapolation because movement can be changed at any instant by players, and make it impossible to predict next position based on previous data.


## Features

The code is a revisited implementation of Quake 3 networking principe. It include some fixes that were introduced since then (in QuakeWorld for example).

All the networking is based on unity's brand new unet networking system. No external assembly are required and run well on desktop and mobile platforms.

All the character movement is based on [Roystan Ross's excellent SuperCharacterController](https://roystanross.wordpress.com/category/unity-character-controller-series/). This is because the unity default character controller isn't deterministic enough to allow rewind-and-replay. Roystan Ross's version is more deterministic and also superior in terms of quality and usability.

## Special thanks

Special thanks to:

 * John Carmack for the Quake3 original source code.
 * PhobicGunner for his [very useful tips on authoritative movement](http://forum.unity3d.com/threads/tips-for-server-authoritative-player-movement.199538/)
 * Roystan Ross for his [excellent SuperCharacterController](https://roystanross.wordpress.com/category/unity-character-controller-series/)
 * [The Unlagged mod of Quake3 networking](http://www.ra.is/unlagged/)
 * Valve for the [documentation on lag compensation in the source engine](https://developer.valvesoftware.com/wiki/Source_Multiplayer_Networking)
 * Also thanks to unity team to providing us with such a great tool