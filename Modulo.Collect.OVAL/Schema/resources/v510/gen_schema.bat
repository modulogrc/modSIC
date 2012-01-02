set xsd2code="c:\Program Files (x86)\Xsd2Code\Xsd2Code.exe"

%xsd2code% oval-definitions-schema.xsd Modulo.Collect.OVAL.Definitions oval-definitions.cs /if- /eit+ /sc+ /xa+ /cu Modulo.Collect.OVAL.Common.XmlSignatures,Modulo.Collect.OVAL.Common /c Array
%xsd2code% independent-definitions-schema.xsd Modulo.Collect.OVAL.Definitions.Independent independent-definitions.cs /if- /eit+ /sc+ /xa+ /cu Modulo.Collect.OVAL.Common,Modulo.Collect.OVAL.Definitions /c Array
%xsd2code% windows-definitions-schema.xsd Modulo.Collect.OVAL.Definitions.Windows windows-definitions.cs /if- /eit+ /sc+ /xa+ /cu Modulo.Collect.OVAL.Common,Modulo.Collect.OVAL.Definitions /c Array
%xsd2code% unix-definitions-schema.xsd Modulo.Collect.OVAL.Definitions.Unix unix-definitions.cs /if- /eit+ /sc+ /xa+ /cu Modulo.Collect.OVAL.Common,Modulo.Collect.OVAL.Definitions /c Array
%xsd2code% linux-definitions-schema.xsd Modulo.Collect.OVAL.Definitions.Linux linux-definitions.cs /if- /eit+ /sc+ /xa+ /cu Modulo.Collect.OVAL.Common,Modulo.Collect.OVAL.Definitions /c Array
%xsd2code% solaris-definitions-schema.xsd Modulo.Collect.OVAL.Definitions.Solaris solaris-definitions.cs /if- /eit+ /sc+ /xa+ /cu Modulo.Collect.OVAL.Common,Modulo.Collect.OVAL.Definitions /c Array
%xsd2code% ios-definitions-schema.xsd Modulo.Collect.OVAL.Definitions.Ios ios-definitions.cs /if- /eit+ /sc+ /xa+ /cu Modulo.Collect.OVAL.Common,Modulo.Collect.OVAL.Definitions /c Array

%xsd2code% oval-system-characteristics-schema.xsd Modulo.Collect.OVAL.SystemCharacteristics oval-system-characteristics.cs /if- /eit+ /sc+ /xa+ /cu Modulo.Collect.OVAL.Common.XmlSignatures,Modulo.Collect.OVAL.Common /c Array
%xsd2code% independent-system-characteristics-schema.xsd Modulo.Collect.OVAL.SystemCharacteristics independent-system-characteristics.cs /if- /eit+ /sc+ /xa+ /cu Modulo.Collect.OVAL.Common,Modulo.Collect.OVAL.SystemCharacteristics /c Array
%xsd2code% windows-system-characteristics-schema.xsd Modulo.Collect.OVAL.SystemCharacteristics windows-system-characteristics.cs /if- /eit+ /sc+ /xa+ /cu Modulo.Collect.OVAL.Common,Modulo.Collect.OVAL.SystemCharacteristics /c Array
%xsd2code% unix-system-characteristics-schema.xsd Modulo.Collect.OVAL.SystemCharacteristics.Unix unix-system-characteristics.cs /if- /eit+ /sc+ /xa+ /cu Modulo.Collect.OVAL.Common,Modulo.Collect.OVAL.SystemCharacteristics /c Array
%xsd2code% linux-system-characteristics-schema.xsd Modulo.Collect.OVAL.SystemCharacteristics.Linux linux-system-characteristics.cs /if- /eit+ /sc+ /xa+ /cu Modulo.Collect.OVAL.Common,Modulo.Collect.OVAL.SystemCharacteristics /c Array
%xsd2code% solaris-system-characteristics-schema.xsd Modulo.Collect.OVAL.SystemCharacteristics.Solaris solaris-system-characteristics.cs /if- /eit+ /sc+ /xa+ /cu Modulo.Collect.OVAL.Common,Modulo.Collect.OVAL.SystemCharacteristics /c Array
%xsd2code% ios-system-characteristics-schema.xsd Modulo.Collect.OVAL.SystemCharacteristics.Ios ios-system-characteristics.cs /if- /eit+ /sc+ /xa+ /cu Modulo.Collect.OVAL.Common,Modulo.Collect.OVAL.SystemCharacteristics /c Array

%xsd2code% oval-results-schema.xsd Modulo.Collect.OVAL.Results oval-results.cs /if- /eit+ /sc+ /xa+ /cu Modulo.Collect.OVAL.Common.XmlSignatures,Modulo.Collect.OVAL.Common,Modulo.Collect.OVAL.Definitions,Modulo.Collect.OVAL.SystemCharacteristics /c Array

