using System;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Generic;

public static class Connection
{
	#region var

	private static String userAgent = "Mozilla/5.0 (Windows NT 6.2; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1667.0 Safari/537.36";
	private static String host = "";
	private static String LastUri = "";
	private static String Null = "0";
	private static Dictionary<String, String> cookieheader_key_value = new Dictionary<string, string>();
	private static CookieContainer cookie = new CookieContainer();
	private static String cookieheader
	{
		get
		{
			String tmp = "";
			if (cookieheader_key_value.Count != 0)
			{
				foreach (KeyValuePair<String, String> kvp in cookieheader_key_value)
					tmp += kvp.Key + "=" + kvp.Value + "; ";
				return tmp.Substring(0, tmp.Length - 2);
			}
			else
				return "";
		}
	}

	#endregion

	#region Cookie

	public static String Cookie_GetValue(String Key)
	{
		foreach (KeyValuePair<String, String> kvp in cookieheader_key_value)
		{
			if (kvp.Key == Key)
				return kvp.Value;
		}
		return "";
	}

	public static void Cookie_SetValue(String Key, String Value)
	{
		cookieheader_key_value.Add(Key, Value);
	}

	public static void Cookie_Remove(String Key)
	{
		String key = "";
		foreach (KeyValuePair<String, String> kvp in cookieheader_key_value)
		{
			if (kvp.Key == Key)
				key = kvp.Key;
		}
		cookieheader_key_value.Remove(key);
	}

	#endregion

	#region Connection

	//Connect to a url with "Get" request and no action after that
	public static bool Get(String url)
	{
		return Connect(url, "", "", ref Null);
	}

	//Connect to a url with "Get" request and get the response after that
	public static bool Get(String url, ref String text)
	{
		return Connect(url, "", "", ref text);
	}

	//Connect to a url with "Get" request (with proxy) and no action after that
	public static bool Get(String url, String proxy)
	{
		return Connect(url, "", proxy, ref Null);
	}

	//Connect to a url with "Get" request (with proxy) and get the response after that
	public static bool Get(String url, String proxy, ref String text)
	{
		return Connect(url, "", proxy, ref text);
	}

	//Connect to a url with "Get", "Post" request and no action after that
	public static bool Post(String url, String post)
	{
		return Connect(url, post, "", ref Null);
	}

	//Connect to a url with "Get", "Post" request and get the response after that
	public static bool Post(String url, String post, ref String text)
	{
		return Connect(url, post, "", ref text);
	}

	//Connect to a url with "Get", "Post" request (with proxy) and get the response after that
	public static bool Post(String url, String post, String proxy, ref String text)
	{
		return Connect(url, post, proxy, ref text);
	}

	//Connect to a url with "Get", "Post" request (and decide to set cookie while request) 
	//(fake decide the cookie header during request) and get the response after that
	private static bool Connect(String url, String post, String proxy, ref String text)
	{
		//fake is disable
		HttpWebRequest request;
		HttpWebResponse response;
		//String ori = "";

		try
		{
			//Connect to url
			//this.form.ct_status(0, url);
			request = (HttpWebRequest)WebRequest.Create(@url);
			request.Accept = "*/*";
			request.Headers.Add("Accept-Language", "zh-tw");
			request.UserAgent = userAgent;
			request.Headers.Add("Accept-Encoding", "deflate");
			if (proxy != "")
				request.Proxy = new WebProxy(proxy);

			if (request.Address.Host != host)
				cookieheader_key_value.Clear();
			host = request.Address.Host;

			if (LastUri != "")
				request.Referer = LastUri;
			LastUri = url;

			request.Headers.Add("Cookie", cookieheader);
			request.AllowAutoRedirect = true;

			//Write to url if there are data to be posted
			if (post != "")
			{
				UTF8Encoding utf8 = new UTF8Encoding();
				Byte[] postdata = utf8.GetBytes(post);
				request.Method = "POST";
				request.ContentType = "application/x-www-form-urlencoded";
				request.ContentLength = postdata.Length;
				using (Stream stm = request.GetRequestStream())
				{
					stm.Write(postdata, 0, postdata.Length);
					stm.Close();
				}
			}

			response = (HttpWebResponse)request.GetResponse();
			if (response != null && response.Headers["Set-Cookie"] != null)
			//	cookie = request.CookieContainer;
			{
				String tmpcook = response.Headers["Set-Cookie"];
				String[] Arraytmpcook = tmpcook.Replace(" ", "").Split(new String[] {";"}, StringSplitOptions.None);
				for (int i = 0; i < Arraytmpcook.Length; i++)
				{
					String[] tmpcook_Key_Value = Arraytmpcook[i].Split('=');
					if (cookieheader_key_value.Count != 0)
					{
						String kvpkep = "";
						bool add = false;
						foreach (KeyValuePair<String, String> kvp in cookieheader_key_value)
						{
							if (kvp.Key == tmpcook_Key_Value[0])
							{
								kvpkep = kvp.Key;
								add = true;
								break;
							}
						}
						if (!add && tmpcook_Key_Value[1] != "/" && tmpcook_Key_Value[1] != "delete" && tmpcook_Key_Value[1] != "\\")
							cookieheader_key_value.Add(tmpcook_Key_Value[0], tmpcook_Key_Value[1]);
						else
						{
							cookieheader_key_value.Remove(kvpkep);
							if (tmpcook_Key_Value[1] != "/" && tmpcook_Key_Value[1] != "delete" && tmpcook_Key_Value[1] != "\\")
								cookieheader_key_value.Add(tmpcook_Key_Value[0], tmpcook_Key_Value[1]);
						}
					}
					else if (tmpcook_Key_Value[1] != "/" && tmpcook_Key_Value[1] != "delete" && tmpcook_Key_Value[1] != "\\")
						cookieheader_key_value.Add(tmpcook_Key_Value[0], tmpcook_Key_Value[1]);
				}
			}


			//"0" define no data to be gotten, otherwise, get data from the stm
			if (text != "0")
			{
				text = "";
				using (Stream stm = response.GetResponseStream())
				{
					StreamReader reader = new StreamReader(stm, Encoding.UTF8);
					while (!reader.EndOfStream)
						text += reader.ReadLine() + "\r\n";
				}
			}
			response.Close();

			return true;

		}
		catch
		{
			return false;
		}
	}

	public static bool Download(string URI, string dest)
	{
		Stream rstm;
		return Download(URI, dest, out rstm);
	}

	public static bool Download(string URI, out Stream rstm)
	{
		return Download(URI, null, out rstm);
	}

	private static bool Download(string URI, string dest, out Stream rstm)
	{
		HttpWebRequest request;
		HttpWebResponse response;

		request = (HttpWebRequest)WebRequest.Create(URI);
		request.Accept = "*/*";
		request.Headers.Add("Accept-Language", "zh-tw");
		request.UserAgent = userAgent;
		request.Headers.Add("Accept-Encoding", "deflate");

		response = (HttpWebResponse)request.GetResponse();
		if (response != null)
		{
			using (Stream stm = response.GetResponseStream())
			{
				if (dest == null)
				{
					CopyStream(stm, out rstm);
					return true;
				}

				byte[] bytesInStream = new byte[8 * 1024];
				int len;
				using (Stream sw = File.OpenWrite(dest))
				{
					while ((len = stm.Read(bytesInStream, 0, (int)bytesInStream.Length)) > 0)
						sw.Write(bytesInStream, 0, len);
				}
			}
			response.Close();
		}

		rstm = Stream.Null;
		return true;
	}

	#endregion

	private static void CopyStream(Stream input, out Stream output)
	{
		output = Stream.Null;
		byte[] buffer = new byte[32768];
		int read;
		while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
		{
			output.Write(buffer, 0, read);
		}
	}
}
