# CS426 HW04  Physics, Textures, and Lights

## JOSEPHINE LEE

## IMPORTANT!!
The initial submission for Assignment 2 **does** work in multiplayer. This submission for HW04 works in multiplayer as well but if it needs to be tested in single-player, please use run **host** version of the game. 

Professor has also mentioned that while the deadline was on Friday 3/13 11:59pm, she said we can continue to submit anytime before class on Monday.

Video Demo [here](https://drive.google.com/file/d/1wwFmIBjkFt9MNB9jqMRdvGop_2cXXTyU/view?usp=sharing)

### Design Additions
The additions from Assignment 2 are: 

* The additional obstacles for both players to overcome in each of their respective areas
* The physics constructs and lights and additional billboard

### Physics Constructs
The two physics constructs added to this game are:

1. Collision knockback when players run into the energy field. They get knocked back and upwards, simulating an explosion-like collision. The energy fields (along with the additional walls) make it more difficult for players to complete their goals and are a purple tint to match the crystals model.
2. Particle system that spawns when a force field is destroyed with the [Q] key, which does not have a limit or cooldown. This particle system is persistent. The particles add a sense of visual accomplishment of destroying the force field and are a purple tint to match the force field.

### Billboard
The billboard with textures added to this game is:

* An "inventory" system representing the coins collected throughout the map. The panel object spawns objects with the texture of a coin, which gets added to the inventory as they are collected. The objects are added incrementally (not shown in video due to time limit) in a horizontal layout. The coin texture of the 2D model matches that of the 3D coin. 

### Lights
The two lights added to this game are:

1. Stationary light emitting from the coins that need to be collected. The lights fit into the design to add an additional goal that the players have to complete and are a gold tint to match the coin model.
2. Rotating light from the crystal sentries that freeze the player in place if they get spotted by it. Players are unable to move or do any actions when they are caught under the spotlight. The rotating lights make it more difficult for players to complete their goals and are a purple tint to match the crystals model.

<br />
<br />

<hr />
<hr />

# ORIGINAL README.md
Group Name: Team 2

Group Members: Alejandra Rios, Elliot Willming, Josephine Lee

## Brief game idea: 
Based on the game Codenames. Grid of 4 x 4 boxes containing computer parts where thieves are hidden within the containers. A hacker provides clues to the firewall to help the firewall locate the thieves. Firewall uses the clues to find the thieves.

## Player Interaction Pattern: 
Multiplayer against game

## Objective: 
Player 1 – Firewall: Find the thieves hidden inside the computer part containers
Player 2 – Hacker: Provide clues for the firewall to locate the thieves 
 
### Serious Objective: 
Learn more about the different components that make up computer systems

## Procedures: 
* System: Generate a grid of 4x4 boxes (with words on top) on a 3D plane
* Player 1 (firewall) can move (8-directional) around the grid
* Player 1 (firewall) can guess a box
* Player 2 (hacker) can see the thieves locations

## Rules: 
* Clues for the correct boxes are randomly generated
* Thieves locations are randomly generated (5 thieves total)
* Player 1 (firewall) loses a life if they guess wrong
* Player 2 cannot reveal directly where the thieves are located
* Player 2 can only give one clue in between guesses
* Clues must be short and concise – no long responses
* Clues do not necessarily have to encompass all the words of the correct answers

## Resources: 
* Only 3 lives are given to the firewall
* Clues are limited

## Non-plain-vanilla procedure/rule:
Asymmetric players

### List of categories
* Inputs (keyboard, mouse, scanner, joystick, microphone)
* Outputs (monitor, speaker, printer, headphone)
* Memory (RAM, SSD, ROM, cache, Hard drives, USB Sticks)
* Boolean Gates (AND, OR, NOT, NAND, NOR)

## Test Question for Serious Objective: Which one of these is not a type of memory?
    A) RAM
    B) ROM
    C) Keyboard

Expected Correct Answer for Serious Objective: Keyboard.
