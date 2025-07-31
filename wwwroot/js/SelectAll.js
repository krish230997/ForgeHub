
window.onload = function () {
  
    var selectAll = document.getElementById("selectAllVendors");

   
    var vendorBoxes = document.getElementsByClassName("vendor-checkbox");


    selectAll.onclick = function () {
     
        for (var i = 0; i < vendorBoxes.length; i++) {
         
            vendorBoxes[i].checked = selectAll.checked;
        }
    };


    for (var i = 0; i < vendorBoxes.length; i++) {
        vendorBoxes[i].onclick = function () {
            var allChecked = true;

        
            for (var j = 0; j < vendorBoxes.length; j++) {
                if (!vendorBoxes[j].checked) {
                    allChecked = false;
                }
            }

  
            if (allChecked) {
                selectAll.checked = true;
            } else {
                selectAll.checked = false;
            }
        };
    }
};
