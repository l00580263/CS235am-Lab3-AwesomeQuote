using Android.App;
using Android.Widget;
using Android.OS;
using System.Xml.Serialization;
using System.IO;

namespace Lab3
{
    [Activity(Label = "Lab3", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        #region Fields
        QuoteBank quoteCollection;
        TextView quotationTextView;
        TextView authorTextView;
        EditText quoteEditText;
        EditText authorEditText;

        const string QUOTE_BUNDLE_KEY = "quote key";
        #endregion



        #region Methods
        protected override void OnCreate(Bundle savedInstanceState)
        {
            // load quotes from bundle
            if (savedInstanceState != null)
            {
                // get string quotes
                string quotes = savedInstanceState.GetString(QUOTE_BUNDLE_KEY);
                // create serializer
                XmlSerializer cereal = new XmlSerializer(typeof(QuoteBank));
                // stringreader
                StringReader reader = new StringReader(quotes);
                // deserializer
                quoteCollection = (QuoteBank)cereal.Deserialize(reader);
            }
            else
            {
                // Create the quote collection and display the current quote
                quoteCollection = new QuoteBank();
                quoteCollection.LoadQuotes();
                quoteCollection.GetNextQuote();
            }

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);
           

            // get quote text view
            quotationTextView = FindViewById<TextView>(Resource.Id.quoteTextView);
            // get author text view
            authorTextView = FindViewById<TextView>(Resource.Id.authorTextView);
            // initialize text views
            UpdateTextViews();

            // Display another quote when the "Next Quote" button is tapped
            var nextButton = FindViewById<Button>(Resource.Id.nextButton);
            nextButton.Click += delegate {
                quoteCollection.GetNextQuote();
                UpdateTextViews();
            };



            // get edittexts
            quoteEditText = FindViewById<EditText>(Resource.Id.quoteEditText);
            authorEditText = FindViewById<EditText>(Resource.Id.authorEditText);

            // add new quote when "add new quote" button is pressed
            var addButton = FindViewById<Button>(Resource.Id.addButton);
            addButton.Click += delegate {
                // add new quote
                var newQuote = new Quote() { Quotation = quoteEditText.Text, Person = authorEditText.Text };

                // stop if edittexts are empty
                if (newQuote.Person.Length < 1 || newQuote.Quotation.Length < 1)
                {
                    return;
                }

                quoteCollection.Quotes.Add(newQuote);
                // display new quote
                UpdateTextViews(newQuote);
                // clear adding edittexts
                quoteEditText.Text = "";
                authorEditText.Text = "";
            };

        }



        void UpdateTextViews()
        {
            // display the current quote
            quotationTextView.Text = quoteCollection.CurrentQuote.Quotation;
            authorTextView.Text = "-" + quoteCollection.CurrentQuote.Person;
        }



        void UpdateTextViews(Quote q)
        {
            // display the passed quote
            quotationTextView.Text = q.Quotation;
            authorTextView.Text = "-" + q.Person;
        }




        protected override void OnSaveInstanceState(Bundle outState)
        {
            // make writer
            StringWriter writer = new StringWriter();
            // set up serializer
            XmlSerializer cereal = new XmlSerializer(quoteCollection.GetType());
            // serialize quote collection
            cereal.Serialize(writer, quoteCollection);
            // get as string
            string quotes = writer.ToString();

            // add to bundle
            outState.PutString(QUOTE_BUNDLE_KEY, quotes);

            base.OnSaveInstanceState(outState);
        }
        #endregion
    }
}

