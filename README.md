## Information

exomia/language support different languages in your application

## Example

```csharp
static void Main(string[] args)
{
	ITranslator translator = new Translator(".", "en");
	translator.Load("example.phrases");
	//translator.Load("example2.phrases");

	Console.WriteLine(translator.Format("here we go... %t", "hello world"));	//here we go... Hello World
	Console.WriteLine(translator.Format("%T", "say", "exomia", "hello world"));	//exomia say: 'Hello World'
	Console.WriteLine(translator.Format("%t", "say2", "exomia"));				//exomia say: 'this is an example'

	Console.ReadKey();
}
```

example.phrases
```
"phrases"
{
	"hello world"
	{
		"en" "Hello World"
		"de" "Hallo Welt"
	}
	"say"
	{
		"en" "{0} say: '{1:T}'"
		"de" "{0} sagt: '{1:T}'"
	}
	"say2"
	{
		"en" "{0} say: '{1:example}'"
		"de" "{0} sagt: '{1:example}'"
	}
	"example"
	{
		"en" "this is an example"
		"de" "dies ist ein Beispiel"
	}
}
```


## Installing

```shell
[Package Manager]
PM> Install-Package Exomia.Language
```

## Changelog

## License

MIT License
Copyright (c) 2018 exomia

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

