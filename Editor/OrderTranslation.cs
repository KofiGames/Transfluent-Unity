﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Pathfinding.Serialization.JsonFx;

namespace transfluent
{
	public class OrderTranslation
	{
		public enum TranslationQuality
		{
			PAIR_OF_TRANSLATORS=3,
			PROFESSIONAL_TRANSLATOR=2,
			NATIVE_SPEAKER=1,
		}
		//group_id, source_language, target_languages, texts, comment, callback_url, max_words [=1000], level [=2], token
		public TextsTranslateResult fullResult;
		public string group_id { get; set; }
		public int source_language { get; set; }
		public int[] target_languages { get; set; }
		public string[] texts { get; set; }
		public string comment { get; set; }

		public string authToken { get; set; }

		//optional
		public int max_words { get; set; }

		[DefaultValue(TranslationQuality.PROFESSIONAL_TRANSLATOR)]
		public TranslationQuality level { get; set; }

		public void Execute()
		{
			IWebService service = new DebugSyncronousEditorWebRequest(); // SyncronousEditorWebRequest
			var containerOfTextIDsToUse = new List<TextIDToTranslateContainer>();
			foreach(string toTranslate in texts)
			{
				containerOfTextIDsToUse.Add(new TextIDToTranslateContainer
				{
					id = toTranslate
				});
			}
			if (level != TranslationQuality.PROFESSIONAL_TRANSLATOR)
			{
				throw new Exception("DEFAULT VALUE NOT WORKING");
			}

			var webserviceParams = new Dictionary<string, string>
			{
				{"source_language", source_language.ToString()},
				{"target_languages", JsonWriter.Serialize(target_languages)},
				{"texts", JsonWriter.Serialize(containerOfTextIDsToUse)},
				{"token", authToken},
				{"level",((int)level).ToString()}
			};
			if(group_id != null)
			{
				webserviceParams.Add("group_id", group_id);
			}
			if (!string.IsNullOrEmpty(comment))
			{
				webserviceParams.Add("comment",comment);
			}
			if (max_words > 0)
			{
				webserviceParams.Add("max_words",max_words.ToString());
			}
			//ReturnStatus status = service.request(RestUrl.getURL(RestUrl.RestAction.TEXTSTRANSLATE) + service.encodeGETParams(webserviceParams));
			ReturnStatus status = service.request(RestUrl.getURL(RestUrl.RestAction.TEXTSTRANSLATE), webserviceParams);
			string responseText = status.text;

			var reader = new ResponseReader<TextsTranslateResult>
			{
				text = responseText
			};
			reader.deserialize();
			fullResult = reader.response;
		}

		[Serializable]
		public class TextIDToTranslateContainer
		{
			public string id;
		}

		[Serializable]
		public class TextsTranslateResult
		{
			public int ordered_word_count;
			public int word_count;
		}
	}
}
