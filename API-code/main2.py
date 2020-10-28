from flask import Flask
from ask_sdk_core.skill_builder import SkillBuilder
from flask_ask_sdk.skill_adapter import SkillAdapter

app = Flask(__name__)
skill_builder = SkillBuilder()
# Register your intent handlers to the skill_builder object

skill_adapter = SkillAdapter(
    skill=skill_builder.create(), skill_id="1", app=app)

@app.route("/api/v1/blueassistant")
def invoke_skill():
    print("here")
    return skill_adapter.dispatch_request()

app.run()