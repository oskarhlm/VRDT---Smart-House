using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Windows.Speech;
using Utils;
using UnityEngine;

namespace Assets
{
    public class VoiceCommands : Singleton<VoiceCommands>
    {
        private KeywordRecognizer _keywordRecognizer;
        private Dictionary<string, Action> _actions = new Dictionary<string, Action>();

        private void Start()
        {
            Debug.Log("voiceeeee");
            _keywordRecognizer = new KeywordRecognizer(new string[1] { "blargh" });
            _keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
            _keywordRecognizer.Start();
        }

        public void AddActions(Dictionary<string, Action> actions)
        {
            _actions = _actions.Union(actions).ToDictionary(x => x.Key, x => x.Value);

            _keywordRecognizer.Dispose();
            _keywordRecognizer = new KeywordRecognizer(_keywordRecognizer.Keywords.Concat(actions.Keys).ToArray());
            _keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
            _keywordRecognizer.Start();
        }

        private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
        {
            Debug.Log(speech.text);
            _actions[speech.text].Invoke();
        }
    }
}
