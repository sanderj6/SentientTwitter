# SentientTwitter
Test Application to gauge Twitter Sentiment

Notes:

The bearer token will need to be added to appsettings.json in order to properly work 

My application combines the Twitter Sample Stream API with Microsoft's Cognitive Services to analyze the sentiment of tweets.

You can view the current trending subject, the top 10 hashtags, and examine the sentiment breakdown for each tweet.

Selecting hashtags adds a second list of tweets filtered by your selection.

I've utilized the Virtualize component to avoid performance issues when dealing with hundreds of thousands of values.

CosmosDB integration has been added, but isn't finished at the time of this ReadMe.
