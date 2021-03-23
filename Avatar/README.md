## Unity client

This is the actual user interface. It should be run alongside the Alexa client on the same machine.

## How it works

It talks with the skill server and the Alexa client with a HTTP-polling-based API, as detailed in [/API-code/README.md](../API-code/README.md). The functionalities are written as in unity components implemented in a modular way. Components which control the behaviour of the model are attached to the avatar themselves. There is a shared face-tracking component which the Player component can pull data from, in order to set model orientation. The bubble component is also shared.

On startup, the `Config` component on `playermover` reads `config.json` from the asset folder, and enables the selected avatar. Components on the avatar can thus take a reference to the `Config` component and pull config data from it, such as skill server address, weather location, etc.

## Unit testing

Unit tests are written in [Assets/Tests](Assets/Tests). Currently there is one test for the video player. In order to run the test you need to open the "Test Runner" window in unity. The test is a "play mode" test, which means that it will start the game just like a user has started it, control the components and assert their states.
