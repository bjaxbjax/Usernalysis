# Usernalysis

This web app analyzes user data modeled to [Random User Generator](https://randomuser.me/).

## Web App Usage

Submit users data to the app either through file upload or pasting the users data into the provided text input field.  Users data must be formatted in JSON.

The set of users must be defined in the `results` property.

Example JSON:

    {
      "results": [
        {
          "gender": "male",
          "name": {
            "title": "mr",
            "first": "rolf",
            "last": "hegdal"
          },
          "location": {
            "street": "123 fake street",
            "city": "new york",
            "state": "new york",
            "postcode": "10001"
          },
          "dob": {
            "date": "1975-11-12T06:34:44Z",
            "age": 42
          }
        },{
          "gender": "female",
          "name": {
            "title": "ms",
            "first": "jean",
            "last": "berry"
          },
          "location": {
            "street": "456 rodeo drive",
            "city": "beverly hills",
            "state": "california",
            "postcode": "90210"
          },
          "dob": {
            "date": "2005-11-12T06:34:44Z",
            "age": 13
          }
        }
      ]
    }

Once submitted, the web app will render charts with the following analyses

1. Percentage female versus male
2. Percentage of first names and last names that start with A‐M versus N‐Z
3. Percentage of people in each state, up to the top 10 most populous states
4. Percentage of females in each state, up to the top 10 most populous states
5. Percentage of males in each state, up to the top 10 most populous states
6. Percentage of people in the following age ranges: 0‐20, 21‐40, 41‐60, 61‐80, 81‐100,
100+
7. Top 10 states with the highest average age

Charts related to states contain choropleth maps as an alternative illustration of the data.  A switch on the upper-left hand corner of these charts controls the toggle between chart and map.

## API

Raw analysis data is available on the following REST API endpoint:

    /api/useranalysis

The endpoint accepts `POST` requests with the JSON-formatted user data in the body of the request or as an uploaded file.

The API can output the analysis in 3 formats:

- plaintext (default)
- json
- xml

Clients can specify the desired format with the request header or the `format` querystring parameter.

###Request header

Set the value for `Accept` to:

- `text/plain` for plaintext
- `application/json` for json
- `application/xml` for xml

Request header format specification takes precedence over the querystring parameter.

###Querystring parameter
Set the value for `format` to:

- `text` for plaintext
- `json` for json
- `xml` for xml

Example:

    https://usernalysis.herokuapp.com/api/useranalysis?format=xml

###Example Ajax Request
    $.ajax({
      url: 'https://usernalysis.herokuapp.com/api/useranalysis',
      dataType: 'json',
      type: 'POST',
      contentType: 'application/json',
      data: <YOUR USER JSON>,
      success: function(response) {
        console.log(response);
      }
    });

## Coding Style
Spacing, indenting, and formatting for this web app relies heavily on Visual Studio's default macros for respective front-end (html, css, js) and back-end (C#) files.

Front-end naming and additional coding styles follow conventions outlined by [w3schools](https://www.w3schools.com/js/js_conventions.asp).  Back-end C# code follows [microsoft](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions).