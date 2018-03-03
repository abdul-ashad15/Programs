
//var myApp = angular.module("myModule", []) //Module

//var myControler = function ($scope) {
//    $scope.message = "Angular JS Tutorial";  //Controller
//};

//myApp.controller("myController", myControler); //Register controller with module


// Or directly we can do like this also


//myApp.controller("myController", function ($scope) {
//    $scope.message = "Angular JS Tutorial";  //Controller
//});


//var myApp = angular
  //                 .module("myModule", [])
    //               .controller("myController", function ($scope) {

                       //var employees = [
                       //    { firstName: "Abdul", lastName: "Ashad1", gender: "Male" },
                       //    {firstName:"Abdul",lastName:"Ashad2",gender:"Male"},
                       //    {firstName:"Abdul",lastName:"Ashad3",gender:"Male"},
                       //    {firstName:"Abdul",lastName:"Ashad4",gender:"Male"},
                       //    {firstName:"Abdul",lastName:"Ashad5",gender:"Male"}
                       //];
                       //$scope.employees = employees;
                       
                       //var countries = [
                       //    {
                       //    name:"INDIA",
                       //    cities : [
                       //        {name : "Bangalore"},
                       //        {name : "Bhubaneswar"},
                       //        {name : "Chennai"}
                       //    ]
                       //    },
                       //    {
                       //        name: "USA",
                       //        cities: [
                       //            { name: "Newyork" },
                       //            { name: "Newyork" },
                       //            { name: "Newyork" }
                       //        ]
                       //    },
                       //    {
                       //        name: "UAE",
                       //        cities: [
                       //            { name: "Dubai" },
                       //            { name: "Abu Dhabi" },
                       //            { name: "Sharjah" }
                       //        ]
                       //    }
                       //];
                       //$scope.countries = countries;

                       //var technologies = [
                       //    { name: "C#", likes: 0, dislikes: 0 },
                       //    { name: "ASP.NET", likes: 0, dislikes: 0 },
                       //    { name: "SQL Server", likes: 0, dislikes: 0 },
                       //    { name: "Angular JS", likes: 0, dislikes: 0 }
                       //];

                       //$scope.technologies = technologies;

                       //$scope.incrementLikes = function (technology) {
                       //    technology.likes++;
                       //}

                       //$scope.incrementDislikes = function (technology) {
                       //    technology.dislikes++;
                       //}

                  // });


// Calling a web API from Angular application
//var app = angular
//          .module("myModule", [])
//          .controller("myController", function($scope,$http,$log){
//              //$http.get('http://localhost:46478/api/students/GetStudents')
//              //Or we can use like below, in the above example we are passing parameter i.e url to $http service get short cut method. 
//              $http({
//                  method: 'GET',
//                  url: 'http://localhost:46478/api/students/GetStudentsnew'
//              })

//              var successCallback = function (response) {
//                  $http.Students = response.data;
//              };

//              var errorCallback = function (response) {

//              };
          
          //$http.get('http://localhost:46478/api/students/GetStudents')
          //.then(function (response) {
          //    $scope.Students = response.data;
          //    //$log.info(response)
          //},
          //function(reason){
          //    $scope.error = reason.data;
          //    //$log.info(reason);
          //});

          //});


var app = angular
        .module("GithubModule", [])
        .controller("GithubController", function ($scope, $http) {

            var onUserComplete = function (response) {
                $scope.user = response.data;
            };
            //alert(user.name);
            var onError = function (reason) {
                $scope.error = "Couln't fetch user information";
            };

            $http.get("https://api.github.com/users/robconery")
                 .then(onUserComplete, onError);
        });


