CS-Connection
=============

It is an simple C# class to perform http or https get and post request.

## Usage

Simple Get Request:

```
String text = "";
Connection.get("http://example.com", ref text);
```

Simple Post Request without response:

```
String text = "";
Connection.post("http://example.com", "key=your_data&key2=your_data");
```

Simple Post Request with response:

```
String text = "";
Connection.post("http://example.com", "key=your_data", ref text);
```

Manage Cookies:

```
//Get Cookie
Connection.Cookie_GetValue("key");
//Set Cookie Value
Connection.Cookie_SetValue("key", "new_val");
//Remove Cookie
Connection.Cookie_Remove("key");
```
