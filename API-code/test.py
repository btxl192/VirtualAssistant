blacklist = ["test.py", "intent_base.py", "__init__.py"]
for file in os.listdir("./handled_intents"):
    if file not in blacklist and file[-3:] == ".py":
        filename = file[:-3]
        intent_name = filename.replace("_",".")
        imported_intent = importlib.import_module(filename)
        intent_instance = getattr(imported_intent, filename)()
        func_mappings[intent_name] = intent_instance.run