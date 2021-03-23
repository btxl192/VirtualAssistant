## Skill server

This is a Flask (Python) http(s) server. [`main.py`](main.py) is the entrypoint, and it contains some rudimentary code that starts the server, handle state request from unity, and delegate different requests into the corrosponding handler in `handled_intents`.

### How Alexa skill works

An Alexa skill consists of different intents, each of which can be invoked via different input patterns with certain placeholders. For example, the "navigation" intent could be:

<pre>
How to get to <i>room</i>
</pre>

where *room* is a placeholder for any name. This pattern matching happens on the Alexa end, and if this intent is matched, we will get a `NavigationIntent` request with the provided *room* as a parameter (aka. slot value). Check out [handled_intents/NavigationIntent.py](handled_intents/NavigationIntent.py) for an example.

Each python file in `handled_intents` represent one intent, and intent handlers can use common functions to return response, send message to unity, or indicate to unity what the user's input is.

The Amazon skill web editor is required in order to add, remove or change the pattern for an intent, in addition to changing the code here, since Amazon does the pattern matching.

### How states are sent to unity

We used a "HTTP polling" approach (due to websocket library not working properly). Unity will periodically request `/msg`, and the handler in `main.py` will return the current state, which consists of what the current response is, whether unity should play video, etc. as well as the timestamp of when the state was updated last.
