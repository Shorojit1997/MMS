$(document).ready(function () {

    function changeStatus(DayId) {
       
        $.post('/Meal/ChangeStatus', { DaysId: DayId }, function (response) {
            if (response.Success) {

            }

        }).fail(function (xhr, status, error) {


        });

    }

    var button = $("#button_for_search");

   
    button.on("click", function () {
        
        var searchBarValue = $("#searchBar").val();
        var cardParentDiv = $("#searchItem");
        var spinner = `<div class="spinner-grow text-muted"></div>
                        <div class="spinner-grow text-primary"></div>
                            <div class="spinner-grow text-success"></div>`; 

        cardParentDiv.empty();
        cardParentDiv.append(spinner); 
      
            
        $.post('/Dashboard/FindMembers', { query: searchBarValue, types: "Name" }, function (response) {
            cardParentDiv.empty();
            for (var i = 0; i < response.length; i++) {
                var messId = $("#MessId").text();
                var cards =`<div class="card">
                        <div class="row g-0">
                            <div class="col-auto">
                                <img src="${response[i].pictureUrl}" alt="Profile Picture" class="rounded-circle" width="80" height="80">
                            </div>
                            <div class="col">
                                <div class="card-body">
                                    <h5 class="card-title">${response[i].name}</h5>
                                    <p class="card-text" style="font-size:12px">${response[i].email}</p>
                                </div>
                            </div>
                           <div class="col-auto d-flex align-items-center">
                                 <div class="justify-content-center">
                                     <div class="dropdown">
                                         <button type="button" class="btn btn-sm btn-primary dropdown-toggle" data-bs-toggle="dropdown">
                                             Add
                                         </button>
                                         <ul class="dropdown-menu">
                                              <li><a href="/Dashboard/AddIntoMess?MessId=${messId}&UserId=${response[i].id}&type=Member" class="btn btn-success">As Member</a></li>
                                              <li><a href="/Dashboard/AddIntoMess?MessId=${messId}&UserId=${response[i].id}&type=Manager" class="btn btn-success">As Manager</a></li>
                                        </ul>
                                       </div>
                                 </div>
                            </div>

                        </div>
                    </div>`
                cardParentDiv.append(cards);
            }
          
        }).fail(function (xhr, status, error) {
            cardParentDiv.empty();
            cardParentDiv.append("<h4>Something happend wrong. Please try again!</h4>");
        });



        return false;
    });
});
