// This script will scan all pages of the input document
// and extract valid email addresses into new PDF document
// Output PDF document will be placed in the same folder
// as input. The name of the output document will be:
// Original filename + "_Extracted_Emails"
// Visit www.evermap.com for more useful JavaScript samples.

var reEmail = /(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))/g;

var strExt = "_Extracted_Emails.pdf";
var strIntro = "Email addresses extracted from document: ";
var strFinal = "Total number of email addresses extracted: " ;

ExtractFromDocument(reEmail,strExt,strIntro,strFinal);

function ExtractFromDocument(reMatch, strFileExt, strMessage1, strMessage2)
{
var chWord, numWords;

// construct filename for output document
var filename = this.path.replace(/\.pdf$/, strFileExt);

// create a report document
try {
    var ReportDoc = new Report();
    var Out = new Object(); // array where we will collect all our emails before outputing them
    
    ReportDoc.writeText(strMessage1 + this.path);
    ReportDoc.divide(1);      // draw a horizontal divider
    ReportDoc.writeText(" "); // write a blank line to output
    var nTotal = 0;
    var nCounter = 0;
    var nLinesPerPages = 60;

    for (var i = 0; i < this.numPages; i++)
    {
        numWords = this.getPageNumWords(i);
        var PageText = "";
        for (var j = 0; j < numWords; j++) {
            var word = this.getPageNthWord(i,j,false);
            PageText += word;
            }
    
        var strMatches = PageText.match(reMatch);
        if (strMatches == null) continue;
        // now output matches into report document
        for (j = 0; j < strMatches.length; j++) 
        {
            ReportDoc.writeText(strMatches[j]);
            nTotal++;
            nCounter++;
            if (nCounter > nLinesPerPages)
            {
                ReportDoc.breakPage();
                nCounter= 0;
            }
        }
    }
    
    ReportDoc.writeText(" "); // output extra blank line
    ReportDoc.divide(1); // draw a horizontal divider
    ReportDoc.writeText(strMessage2 + nTotal);
    
    // save report to a document
    ReportDoc.save(
        {
        cDIPath: filename
        });

}
catch(e)
{
app.alert("Processing error: "+e)
}
    
} // end of the function
    




