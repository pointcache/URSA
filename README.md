
![https://github.com/pointcache/URSA/wiki/Home/](http://i.imgur.com/WNNeEoa.png)
[![GitHub issues](https://img.shields.io/badge/thermal-underpants-brightgreen.svg)](https://github.com/pointcache/URSA/issues)  [![GitHub issues](https://img.shields.io/github/issues/pointcache/URSA.svg)](https://github.com/pointcache/URSA/issues)  ![License MIT](https://img.shields.io/badge/license-MIT-green.svg)  ![tested on unity](https://img.shields.io/badge/unity-2017.1-blue.svg)  


# [Deprecated, unmaintained, silly]


# Open tutorials folder in the project, it has pretty good examples.

URSA is an ECS framework integrated into Unity.
It doesn't use a separate backend, and works on top of unity components in a very simple manner.
URSA streamlines typically complex gamedev tasks like managing initialization and serialization.
Being an ECS it decouples systems and allows data driven approach.

# ECS

URSA is not a full ECS, it has severe limitations over pure code ECS, however it has tremendous benefits in other areas.
Ursa implements concept of a Component, Entity and System, in a way a typical ECS does.
Components store data, and preferable have no logic.
Entity is a top level object identifier, accessible from any component, disregarding hierarchy.
System is the actual logic, it iterates over components each frame and performs actions.
Currently URSA does not support dynamic AddComponent<> serialization.
For save to work correctly with Entities they have to have assigned id's which happens on edit time.
This may be worked on in the future.

Everything in URSA revolves around serialization, the ability to save any individual entity, or group of them, even nested.
This creates interesting opportunities, for example inventory could be represented by literal GameObject entities,
attached to a Backpack component, when you save the backpack it will save every item in it.


However it's made completely on top of unity, without a backend.
This is meant for people who like me prefer to work with editor and inspectors to speed up the process.

An overview of classes and workflow.

The main workhorse of ursa is Entity, its a concrete class and a concept.
If you have an object that represents an enemy, you put an Entity scipt on top of it.
This will allow any ursa system to treat a whole bunch of components that may be deep inside hierarchy like a list.
The power of it in that components become unaware of their place inside the hierarchy, and they clearly know
whos the root. If a system polls for `Enemy` component in an entity, and then checks if it has a `Sword` the position of the 
sword component is irrelevant, and the search is highly optimized compared to recursive search.

URSA's systems iterate through all components in the scene, which they are interested in. 
This is done through the `Pool` class, like so:

```csharp

void OrderedUpdate(){
  for(int i = 0; i < Pool<Enemy>.Count; i++) {
    var enemy = Pool<Enemy>.Components[i];
    if(!enemy.GetEntityComponent<Sword>()) {
      LaughAt(enemy);
      }
  }
  
```

Pool is a static way to access all `ComponentBase` derrived runtime objects, which is very fast.

`Pool<GameCamera>.First` will give you the first component in the list, very convenient for single objects.


# Initialization control

One of the best features of this framework, it streamlines unity systems stacks control.
You are given two easy to use stacks - global and local, where global are top level systems
audio,configs,graphics controllers and so on, and local are your per scene game logic.
Ursa has its own internal initialization sequence into which you can hook your own logic.

Example would be what happens when you first launch the game
1. Load the global data - configs, player profiles
2. Start up URSA's internal systems
3. Enable your custom systems in an order
4. Start loading local systems.

Now when you need to switch scene, you do it through ECSController,
and every time you it will launch local initialization sequence:

1. Load any data you want (savegame for example)
2. Find and activate local systems in order.
3. Raise an event that everything is ready for gameplay (activate ui here for example)

The global local stuff is also made in such way that you can start playing from any scene, given 
it had a global stack in it, and leave those stacks in other scenes without worrying that it will break something.
This means convenience and fast testing.


# Serialization

Ursa provides means to fully save the game in one click. This is not "free" as it requires you to follow
certain workflow. 
Ursa allows you to save all Entities in the scene, with all its components data serialized.
Ursa serializes cross component references as well.

Ursa has `Blueprints` which are small save files you can make from any Entity group (nested as well).
This allows you to create player loadouts , npc inventories, user made content and other things.

Ursa requires you to use prefabs, every prefab you make an Entity is then processed by a database, 
assigned unique ids and is ready to be saved on runtime. If an Entity exists in a game, it has to have a backing prefab.
That is how ursa works. 

This is a bad and a good thing.
The bad is that you are not flexible enough to create ANY runtime generated content you want, and then serialize it.
Think of it like this - you cant (yet) create on runtime a Sword and add FireDamage component to it, and then save.
You can however have a Sword entity, and literally parent to it a FireDamage entity (yes a FireDamage prefab!), and make the sword
use that fire damage. Then both entities will properly serialize (sounds very stupid i know).


URSA was used in several prototypes and is currently used in a production game:


![](https://c1.iggcdn.com/indiegogo-media-prod-cld/image/upload/c_limit,w_620/v1477713096/j0fnw74zujwptizm78k4.png)

![](http://i.imgur.com/MfTaAAy.png)


![](http://i.imgur.com/lLYESg5.jpg)

http://i.imgur.com/VYD6uGw.gifv

![](http://i.imgur.com/hZYIN0I.png)

http://i.imgur.com/Bpzpoy4.gifv

![](http://i.imgur.com/dqnb42R.png)

http://i.imgur.com/q54Vr5N.gif

# Todo

The most lacking feature currently is flexible AddComponent and its proper serialization.
Which probably is not the hardest thing to achieve.

URSA dependencies:
* FullSerializer


