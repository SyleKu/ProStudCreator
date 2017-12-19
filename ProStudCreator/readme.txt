This readme explains how to use this package. For support send your mails
to simon.felix@fhnw.ch or via Twitter to @deiruch.



// returns "en", "de" and null
var language1 = i4Ds.LanguageToolkit.LanguageDetector.DetectLanguage("what language is this?");
var language2 = i4Ds.LanguageToolkit.LanguageDetector.DetectLanguage("in welcher sprache ist dieser satz geschrieben?");
var language3 = i4Ds.LanguageToolkit.LanguageDetector.DetectLanguage("asdfgioazgosdzgsogzsodguzcxkljbhlasdjkfh");


// this returns a sentiment from 0.0 (negative) to 1.0 (positive)
var sentiment1 = i4Ds.LanguageToolkit.SentimentAnalyzer.GetSentiment("this is great");
var sentiment2 = i4Ds.LanguageToolkit.SentimentAnalyzer.GetSentiment("this sucks");