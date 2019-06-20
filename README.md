## Information

exomia/language support different languages in your application

![](https://img.shields.io/github/issues-pr/exomia/language.svg)
![](https://img.shields.io/github/issues/exomia/language.svg)
![](https://img.shields.io/github/last-commit/exomia/language.svg)
![](https://img.shields.io/github/contributors/exomia/language.svg)
![](https://img.shields.io/github/commit-activity/y/exomia/language.svg)
![](https://img.shields.io/github/languages/top/exomia/language.svg)
![](https://img.shields.io/github/languages/count/exomia/language.svg)
![](https://img.shields.io/github/license/exomia/language.svg)

## Installing

```shell
[Package Manager]
PM> Install-Package Exomia.Language
```

## Example

```csharp
static void Main(string[] args)
{
	ITranslator translator = new Translator(".", "en");
	translator.Load("example.phrases");
	//translator.Load("example2.phrases");

	Console.WriteLine(translator.Format("here we go... %t", "hello world"));	//here we go... Hello World
	Console.WriteLine(translator.Format("%T", "say", "exomia", "hello world"));	//exomia say: 'Hello World'
	Console.WriteLine(translator.Format("%t", "say2", "exomia"));			//exomia say: 'this is an example'

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