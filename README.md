# Localization 
A suit of libs to localize C# and WPF project easily base on file format you choose.

__Replace the archived [TranslateMe](https://github.com/codingseb/TranslateMe) lib__

|Nuget|Version|
|---|---|
|CodingSeb.Localization|[![NuGet Status](http://img.shields.io/nuget/v/CodingSeb.Localization.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/CodingSeb.Localization/)|
|CodingSeb.Localization.JsonFileLoader|[![NuGet Status](http://img.shields.io/nuget/v/CodingSeb.Localization.JsonFileLoader.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/CodingSeb.Localization.JsonFileLoader/)|
|CodingSeb.Localization.WPF|[![NuGet Status](http://img.shields.io/nuget/v/CodingSeb.Localization.WPF.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/CodingSeb.Localization.WPF/)|

## The differents parts of the project
The library is composed of 3 parts :  

  1. The part "Core" : Nuget "CodingSeb.Localization" Contains the dictionnary of all translations for a "TextId" in C#.  
	
  2. A part "FileLoader" : Allow to open a type ofile that contains translations to load in the dictionnary of the "Core" part. By default a loader to load localization files in JSON is provided "CodingSeb.Localization.JsonFileLoader".  
	
  3. The part to translate (localize) XAML (WPF) "CodingSeb.Localization.WPF". Provide a `Tr` markup and some converters to use in Bindings. It use the "Core" in backend.  

## Installation

### With Nuget

__For Simple C# projects__
```
PM> Install-Package CodingSeb.Localization.JsonFileLoader
```  
or
```
PM> Install-Package CodingSeb.Localization
``` 
and implement your own FileLoader.

__For WPF projects__  
Add this :
```
PM> Install-Package CodingSeb.Localization.WPF
```

## use it in C# :

__To localize a text__

```csharp
using CodingSeb.Localization
/// ...

// To translate a text in the current language
// Loc.Tr("TextId");
Loc.Tr("SayHello");
// To show a default text if the text is not localized in the current language
// Loc.Tr("TextId", "DefaultText")
Loc.Tr("SayHello", "Non traduit");
// To Translate a text in fixed language
// Loc.Tr("TextId","DefaultText" "LanguageId");
Loc.Tr("SayHello", null, "fr");

```

__To Change the current language__
```csharp
Loc.Instance.CurrentLanguage = "en";
Loc.Instance.CurrentLanguage = "fr";
Loc.Instance.CurrentLanguage = "es";
// ...
// To get availables languages
Collection<string> languages = Loc.Instance.AvailableLanguages;
```

### use it In XAML (WPF) :
(no xmlns needed Tr Markup is available as soon as CodingSeb.Localization.WPF is in project's references)

__Simple localization with the Markup `Tr`__
```xml
<TextBlock Text="{Tr 'SayHello'}"/>
<TextBlock Text="{Tr 'SayHello', DefaultText='Not localized'}"/>
<TextBlock Text="{Tr 'SayHello', LanguageId='fr'}"/>
```

_In general use XML escape to escape special characters. For single quote use ```[apos]``` to escape. XML escape does'nt work in this case for inline Tr markup. Or use the following format : _

```xml
<!-- textId can be automatically calculate (with x:Name and the context of the element) -->
<Label x:Name="lbMyLabel" >
  <Label.Content>
    <Tr DefaultText="Text with a ' here" />
  </Label.Content>
</Label>
```

__To Translate with Bindings__

```xml
<!-- To use the Binding as flexible TextId -->
<TextBlock Text="{Binding MyPropertyAsTextId, Converter={TrTextIdConverter DefaultText='Not localized'}"/>
<!-- or (for enum for example) -->
<TextBlock Text="{Binding MyPropertyAsPartOfTextId, Converter={TrTextIdConverter TextIdStringFormat='MyEnum{0}', DefaultText='Not localized'}"/>

<!-- To use the Binding as flexible LanguageId -->
<TextBlock Text="{Binding MyPropertyAsLanguageId, Converter={TrLanguageIdConverter TextId='SayHello'}" />

<!-- To use the Binding as a value to inject in the localized text -->

<!-- The translation would be "Hello {0}" or "Bonjour {0}"  -->
<TextBlock Text="{Binding FirstName, Converter={TrStringFormatConverter TextId='SayHello'}" />

<!-- The translation would be "Hello {0} {1}" or "Bonjour {0} {1}"  -->
<TextBlock>
    <TextBlock.Text>
        <MultiBinding Converter="{TrStringFormatConverter TextId='SayHello'}">
            <Binding Path="FirstName" />
            <Binding Path="LastName" />
        </MultiBinding>
    </TextBlock.Text>
</TextBlock>

```

*Remark : By default the translation made in the XAML are automatically updated when current language changed*

__To Change the current language from the xaml__

```xml
<!-- to add in the root tag of the xaml file  : 
xmlns:loc="clr-namespace:Localization;assembly=Localization" -->
<ComboBox ItemsSource="{Binding AvailableLanguages, Source={x:Static loc:Loc.Instance}}"
          SelectedItem="{Binding CurrentLanguage, Source={x:Static loc:Loc.Instance}}"/>

```

## OK But... ...How I define my translations ?

With the default JsonFileLoader, Translations are defined in JSON files with the extension "*.loc.json".

Here an example :

```json
{
  "LanguageName": {
    "en": "English",
    "es": "Español",
    "fr": "Français"
  },
  "[TranslateMe.Examples.MainWindow].lblCurrentLanguage[Label].Content": {
    "en": "Current language",
    "es": "Lenguaje actual",
    "fr": "Langue courrante"
  },
  "[TranslateMe.Examples.MainWindow].lblHelloInCurrentLanguage[Label].Content": {
    "en": "Hello",
    "es": "Hola",
    "fr": "Bonjour"
  },
  "HelloInCurrentLanguage": {
    "en": "Hello in the current language",
    "es": "Hola en la lengua actual",
    "fr": "Bonjour dans la langue actuelle"
  },
  "[TranslateMe.Examples.MainWindow].lblHelloInCurrentLanguage[Label].ToolTip": {
    "en": "In english",
    "es": "En español",
    "fr": "En français"
  }
}
```

It's also possible to create a hierarchy :

```json
{
    "AppName": {
        "MainMenu": {
          "FileMenuItem": {
            "Header": {
              "en": "_File",
              "fr": "_Fichier"
            },
            "NewMenuItem": {
              "Header": {
                "en": "_New",
                "fr": "_Nouveau"
              }
            },
            "OpenMenuItem": {
              "Header": {
                "en": "_Open",
                "fr": "_Ouvrir"
              }
            },
            "..."
        }
    }
}
```

To use like this : 

```csharp
Loc.Tr("AppName.MainMenu.FileMenuItem.Header");
Loc.Tr("AppName.MainMenu.FileMenuItem.NewMenuItem.Header");
Loc.Tr("AppName.MainMenu.FileMenuItem.OpenMenuItem.Header");
```

or like this in XAML :
```xml
<Menu>
    <MenuItem Header="{Tr 'AppName.MainMenu.FileMenuItem.Header', DefaultText='_File'}">
        <MenuItem Header="{Tr 'AppName.MainMenu.FileMenuItem.NewMenuItem.Header', DefaultText='_New'}" 
                  Command="ApplicationCommands.New" />
        <MenuItem Header="{Tr 'AppName.MainMenu.FileMenuItem.OpenMenuItem.Header', DefaultText='_Open'}" 
                  Command="ApplicationCommands.Open" />
        <!-- ... -->

``` 

And to load these files :

```csharp
using CodingSeb.Localization.Loaders;
// You need first to add the specific fileLoader 
LocalizationLoader.Instance.FileLanguageLoaders.Add(new JsonFileLoader());

// ...

And then you can add your localization file
LocalizationLoader.Instance.AddFile(@"PathToTheFile\Example1.loc.json");
// or load directly a directory with multiple "*.loc.json" files.
LocalizationLoader.Instance.AddDirectory(@"PathToTheDirectory");
```

So you can change the text of your app or translate it in a new language without recompile all your application.

```csharp
// or you can also load a translation by code (textId, languageId, value)
LocalizationLoader.Instance.AddTranslation("SayHello", "en", "Hello" );
LocalizationLoader.Instance.AddTranslation("SayHello", "es", "Hola" );
LocalizationLoader.Instance.AddTranslation("SayHello", "fr", "Bonjour" );
```

### Implement your own file format
If you want to support an other format than json, you can create your custom FileLanguageLoader.
Simply create a class that implement the ILocalizationFileLoader interface and add an instance of your class in the LocalizationLoader :

```csharp
LocalizationLoader.Instance.FileLanguageLoaders.Add(new YouCustomClassImplementingILocalizationFileLoader());
```

## Find Missing Translations
You can activate an option to be notify when a translation is missing. 

```csharp
// with all TextId and LanguageId that are missing when you trying to translate them.
Loc.Instance.LogOutMissingTranslations = true;
Loc.Instance.MissingTranslationFound += Loc_MissingTranslationFound;
```

If you want to log it automatically in a json file you can also use the class `JsonMissingTranslationsLogger` in the "CodingSeb.Localization.JsonFileLoader" package.

```csharp
JsonMissingTranslationsLogger.EnableLogFor(Loc.Instance);
```

## Tr and WPF Styles
The `Tr`markup is usable in Styles. but if a Trigger us used the `Tr` markup only works if used in static `<Tr IsDynamic=False ...`.
In dynamic mode the `Tr`markup create in backend a Binding and do not allow to be modified by a Datatrigger.
To do a localization in a same manner way, prefer to use a binding with a `TrTextIdConverter` in place of a trigger.
