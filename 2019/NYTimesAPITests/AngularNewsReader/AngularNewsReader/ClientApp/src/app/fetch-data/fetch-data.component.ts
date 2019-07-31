import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
  //'https://api.nytimes.com/svc/search/v2/articlesearch.json?q=election&api-key=5Xyt4lufFlMjQm7Eg3tObKDNPPwvTyAy'
  //NY Times API Call
export class FetchDataComponent {
  public articles: TimesArticle[] = [];


  public page: number = 0;
  public itemRange: string = "?-?";

  public itemCount: number = 15;
  private http: HttpClient;

  public updating: boolean = false;
   
  /*
  updateResults() {
    this.updating = true;
    this.http.get('https://api.nytimes.com/svc/search/v2/articlesearch.json?q=election&page=' + this.page + '&api-key=5Xyt4lufFlMjQm7Eg3tObKDNPPwvTyAy').subscribe(result => {
      var results = result as TimesData;
      var responses = results.response as TimesResponse;
      var timesArticles = responses.docs as TimesArticle[];
      this.articles = timesArticles;
      console.log("Responded");
      this.updating = false;
    }, error => console.error(error));

  }*/
  // The NY Times API does not support requesting more than 10 items, so we're gonna have to get creative!
  // We request two blocks of 10 items, comprising two pages, then provide 15 of those results at a time.
  public firstArticles: TimesArticle[] = [];
  public secondArticles: TimesArticle[] = [];
  private finishedFirstArticles: Boolean = false;
  private finishedSecondArticles: Boolean = false;
  updateResults() {
    this.updating = true;
    this.finishedFirstArticles = false;
    this.finishedSecondArticles = false;
    this.http.get('https://api.nytimes.com/svc/search/v2/articlesearch.json?q=election&page=' + this.page + '&api-key=5Xyt4lufFlMjQm7Eg3tObKDNPPwvTyAy').subscribe(result => {
      var results = result as TimesData;
      var responses = results.response as TimesResponse;
      var timesArticles = responses.docs as TimesArticle[];
      this.firstArticles = timesArticles;
      console.log("Responded");
      this.updating = false;
      this.finishedFirstArticles = true;
      this.http.get('https://api.nytimes.com/svc/search/v2/articlesearch.json?q=election&page=' + (this.page + 1) + '&api-key=5Xyt4lufFlMjQm7Eg3tObKDNPPwvTyAy').subscribe(result => {
        var results = result as TimesData;
        var responses = results.response as TimesResponse;
        var timesArticles = responses.docs as TimesArticle[];
        //this.articles = timesArticles;
        this.secondArticles = timesArticles;
        this.updating = false;
        this.finishedSecondArticles = true;
        this.aggregateResults();
      }, error => console.error(error));
    }, error => console.error(error));
    
    
  }

    // shamelessly "borrowed" from an article on Stackoverflow
    isEven(n) {
      return n == parseFloat(n) ? !(n % 2) : void 0;
    }

  // Depending on whether we're on an odd or even page, we provide the first or last 15 results
  aggregateResults() {
    if (!this.finishedFirstArticles || !this.finishedSecondArticles) {
      return;
    }
    var aggregateArticles = this.firstArticles.concat(this.secondArticles);
    var outputArticles = [];
    if (this.isEven(this.page)) {
      //use first 15
      for (var i = 0; i < 15; i++) {
        outputArticles.push(aggregateArticles[i]);
      }
    } else {
      //use last 15
      for (var i = 5; i < 20; i++) {
        outputArticles.push(aggregateArticles[i]);
      }
    }
    var articleCount = outputArticles.length;
    for (var i = 0; i < articleCount; i++) {
      var article = outputArticles[i];
      var media = article.multimedia;
      var thumb;
      if (media) {
        var imgCount = media.length;
        for (var j = 0; j < imgCount; j++) {
          if (media[j].subtype === "thumbnail") {
            thumb = media[j].url;
          }
        }
      }
      if (thumb) {
        article.thumbnail = "https://static01.nyt.com/" + thumb;
      }
      
    }
    this.articles = outputArticles;
  }
  
  prevPage() {
    if (this.page > 0) {
      this.page--;
      this.updateResults();
      this.updateItemRange();
    }
  }
  nextPage() {
    if (this.page < 100) {
      this.page++;
      this.updateResults();
      this.updateItemRange();
    }
  }

  //Just for a nice display, we can show the index ranges of articles here.
  updateItemRange() {
    this.page;
    var startRange = (this.page * this.itemCount) + 1;
    var endRange = startRange + this.itemCount - 1;
    this.itemRange = startRange + "-" + endRange;
  }

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http = http;
    this.updateResults();
    this.updateItemRange();
  }
}

interface TimesData {
  copywrite: String;
  response: TimesResponse;
  status: String;
}
interface TimesResponse {
  docs: TimesArticle[];
  meta: Object;
}

interface TimesArticle {
  _id: string;
  abstract: string;
  blog: Object;
  byline: Object;
  document_type: string;
  headline: Object;
  keywords: Object;
  lead_paragraph: string;
  multimedia: Object;
  news_desk: string;
  print_page: string;
  pub_date: string;
  section_name: string;
  snippet: string;
  source: string;
  type_of_material: string;
  uri: string;
  web_url: string;
  word_count: string;
  thumbnail: string;

}
