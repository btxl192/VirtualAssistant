from .intent_base import *
from googletrans import Translator
import epitran

class TranslatorIntent(intent_base):

    lang_mapping = {'french': 'fra-Latn'}

    def action(self, handler_input):
        dest_lang = get_slot_value(handler_input, "destLang").lower()
        text = get_slot_value(handler_input, "text")
        
        self.user_input = f"User asked to translate {text} to {dest_lang}"
        
        if dest_lang in self.lang_mapping:
            t = Translator()           
            translated_text = t.translate(text, dest_lang, 'en').text
            
            e = epitran.Epitran(self.lang_mapping[dest_lang])
            phoneme = e.transliterate(translated_text)
            
            self.ignore_dismissal_msg = True
            self.response = f'<phoneme alphabet="ipa" ph="{phoneme}">{translated_text}</phoneme>.'            
            self.override_speech = phoneme
        else:
            self.response = "That language is not supported"
        