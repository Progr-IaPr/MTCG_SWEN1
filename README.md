# MTCG_SWEN1
My semester project for SWEN1

MTCG-Protokoll:

Designs/Failures & Selected Solutions:

Sehr viel Planung steckte nicht hinter meinen Design-Vorgängen, weswegen ich meine Datenbank, meine Klassen
und generell meine Herangehensweise öfter umgearbeitet und verändert habe, als ich denken kann. Im Nachhinein
wäre ein wenig mehr Planung wohl sinnvoll gewesen. Soweit ich das beurteilen kann, habe ich eine so einfache
Lösung wie möglich ausgearbeitet. Allerdings hatte ich lange Schwierigkeiten das Prinzip des REST-based HTTP servers
zu verstehen und habe die meiste Zeit damit vertrödelt immer wieder dort neu anzusetzen.

Schlussendlich (und wie ich feststellen musste viel zu spät) habe ich mich entschlossen die Implementierung heranzuziehen, 
die wir im Unterricht ausgearbeitet haben und auch da habe ich ein bisschen gebraucht, bis ich wusste wie ich damit weiterarbeiten kann.

Am Ende habe ich einfach jeden Pfad einzeln abgearbeitet, was vermutlich nicht die schönste Art der Implementierung war, aber es funktioniert.
Leider habe ich es nicht geschafft, alle Funktionalitäten zu implementieren, z.B fehlen mir das Battle komplett (auch Trading & das UniqueFeature fehlen).

Alle wichtigen Funktionen habe ich in einem File "Database.cs". Sehr um Struktur habe ich mich nicht gekümmert, 
da es mir in erster Linie wichtig war so viele Funktionalitäten wie möglich noch einzubauen.

Lessons Learned:

1. Frag um Hilfe, wenn du sie brauchst, selbst wenn du dir dabei blöd vorkommst. -> Leider nach wie vor etwas, dass ich lernen muss

2. Da es das erste Projekt ist, das ich in C# geschrieben habe, würde ich sagen, dass ich viel Zeit damit verbracht habe, herauszufinden wie 
ich einzelne Funktionen schreibe und welche build-in Funktionen es gibt. Auch an die Syntax habe ich mich erst gewöhnen müssen.

3. Auch war es das erste Mal, dass ich mit einer postresql Datenbank gearbeitet habe und es hat viel Herumspielen und Googlen von meiner Seite
erfordert, damit ich mich nicht nur zurechtfinde, sondern auch erfolgreich mit ihr arbeiten kann. Es nicht gerade wenig Zeit in Anspruch genommen.

Unit Testing Decisions:

Wenn ich ehrlich bin, habe ich mir nicht allzu viel Gedanken über meine Unit Tests gemacht. Eine große Auswahl welche Funktionen ich jetzt testen will hatte ich auch nicht, nachdem einige Funktionalitäten fehlen.

Unique Feature:

Nachdem das einer der Dinge ist, die mir in meiner Lösung fehlen, kann ich dazu nicht mehr sagen als: Ich habe kein Unique Feature implementiert.

Tracked Time:

Ich habe nicht gerade die Zeit gestoppt, aber ich weiß in den ersten 3 Wochen bin ich sozusagen an dem HTTP-server verzweifelt bis in in Woche 4 endlich
herausgefunden habe, wie ich am besten weiterarbeiten kann. Gearbeitet habe ich nach der Arbeit meistens von 19-23/24 Uhr. An 2 Wochenenden bin ich mit nur wenigen Pausen
quasi dauernd an meinem Projekt gesessen.

Link to GIT:

https://github.com/Progr-IaPr/MTCG_SWEN1

