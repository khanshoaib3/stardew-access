import wrapper

speech = wrapper.Speech

speech.Initialize(self=speech)

speech.Say(self=speech, text="hello", interrupt=False)

speech.Close(self=speech)