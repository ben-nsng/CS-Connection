CS-Connection
=============

It is an simple C# class to perform http or https get and post request.

## Usage

Connection.get and Connection.post return whether the connection is success.

Simple Get Request:

```
String text = "";
Connection.Get("http://example.com", ref text);
```

Simple Post Request without response:

```
String text = "";
Connection.Post("http://example.com", "key=your_data&key2=your_data");
```

Simple Post Request with response:

```
String text = "";
Connection.Post("http://example.com", "key=your_data", ref text);
```

Download file with specify destination:

```
Connection.Download("http://example.com/file", "c:/localpath/");
```

Download file with using stream to get the file content:

```
Stream stm;
Connection.Download("http://example.com/file", out stm);
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


