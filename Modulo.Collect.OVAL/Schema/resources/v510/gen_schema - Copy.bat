set xsd2code="c:\Program Files (x86)\Xsd2Code\Xsd2Code.exe"

%xsd2code% ios-definitions-schema.xsd Modulo.Collect.OVAL.Definitions.Ios ios-definitions.cs /if- /eit+ /sc+ /xa+ /cu Modulo.Collect.OVAL.Common,Modulo.Collect.OVAL.Definitions /c Array

%xsd2code% ios-system-characteristics-schema.xsd Modulo.Collect.OVAL.SystemCharacteristics.Ios ios-system-characteristics.cs /if- /eit+ /sc+ /xa+ /cu Modulo.Collect.OVAL.Common,Modulo.Collect.OVAL.SystemCharacteristics /c Array


