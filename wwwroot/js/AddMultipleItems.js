 
            const tabPanes = document.querySelectorAll('.tab-pane');
            const steps = document.querySelectorAll('.step-nav li');

                   function goToStep(stepIndex) {
            tabPanes.forEach((pane, idx) => {
                pane.classList.remove('show', 'active');
                if (idx === stepIndex) {
                    pane.classList.add('show', 'active');
                }
            });

            steps.forEach((step, idx) => {
                step.classList.remove('active', 'completed');
                if (idx < stepIndex) step.classList.add('completed');
                if (idx === stepIndex) step.classList.add('active');
            });

    
            if (stepIndex === 1) {
                const indentNoVal = document.querySelector('input[asp-for="IndentNo"]').value;
                document.getElementById('indentNoDisplay').value = indentNoVal;
            }
        }

            
      

   
        let rfqItems = [];
        let editIndex = -1;

        function addItem() {
            const item = {
            RFQLineNo: document.getElementById("lineNo").value,
        ItemNo: document.getElementById("itemNo").value,
        ItemName: document.getElementById("itemName").value,
        ReqQty: parseInt(document.getElementById("reqQty").value),
        UOM: document.getElementById("uom").value,
        ReqDeliveryDate: document.getElementById("deliveryDate").value,
        DeliveryLocation: document.getElementById("deliveryLocation").value,
        Description: document.getElementById("description").value,
        FactoryCode: document.getElementById("factoryCode").value
            };

        

        if (editIndex === -1) {
            rfqItems.push(item);
            } else {
            rfqItems[editIndex] = item;
        editIndex = -1;
        document.getElementById("addItemBtn").innerText = "Add Item";
            }

        document.getElementById("RFQItemsJson").value = JSON.stringify(rfqItems);

        renderItemsTable();
        resetItemForm();
        }


        function editItem(index) {
            const item = rfqItems[index];

        document.getElementById("lineNo").value = item.RFQLineNo;
        document.getElementById("itemNo").value = item.ItemNo;
        document.getElementById("itemName").value = item.ItemName;
        document.getElementById("reqQty").value = item.ReqQty;
        document.getElementById("uom").value = item.UOM;
        document.getElementById("deliveryDate").value = item.ReqDeliveryDate;
        document.getElementById("deliveryLocation").value = item.DeliveryLocation;
        document.getElementById("factoryCode").value = item.FactoryCode;
        document.getElementById("description").value = item.Description;

        editIndex = index;
        document.getElementById("addItemBtn").innerText = "Update Item";
        }


        function removeItem(index) {
            if (confirm("Are you sure you want to delete this item?")) {
            rfqItems.splice(index, 1);
        renderItemsTable();
            }
        }

        function resetItemForm() {
            document.getElementById("lineNo").value = "";
        document.getElementById("itemNo").value = "";
        document.getElementById("itemName").value = "";
        document.getElementById("reqQty").value = "";
        document.getElementById("uom").value = "";
        document.getElementById("deliveryDate").value = "";
        document.getElementById("deliveryLocation").value = "";
        document.getElementById("factoryCode").value = "";
        document.getElementById("description").value = "";

        editIndex = -1;
        document.getElementById("addItemBtn").innerText = "Add Item";
        }

        function renderItemsTable() {
            const tableBody = document.getElementById("rfqItemsTableBody");
        tableBody.innerHTML = "";

            rfqItems.forEach((item, index) => {
                const row = document.createElement("tr");
        row.innerHTML = `
        <td>${item.RFQLineNo}</td>
        <td>${item.ItemNo}</td>
        <td>${item.ItemName}</td>
        <td>${item.ReqQty}</td>
        <td>${item.UOM}</td>
        <td>${item.ReqDeliveryDate}</td>
        <td>${item.DeliveryLocation}</td>
        <td>${item.FactoryCode}</td>
        <td>${item.Description}</td>
        <td>
            <button type="button" class="btn btn-sm btn-warning" onclick="editItem(${index})">Edit</button>
            <button type="button" class="btn btn-sm btn-danger ms-1" onclick="removeItem(${index})">Remove</button>
        </td>
        `;
        tableBody.appendChild(row);
            });
        }


        function prepareAndSubmitForm() {
            const hiddenField = document.getElementById("RFQItemsJson");
        hiddenField.value = JSON.stringify(rfqItems);
        console.log("Final Items JSON:", hiddenField.value);

        if (!rfqItems.length) {
            alert("Please add at least one item to RFQ.");
        return;
            }

        document.getElementById("rfqForm").submit();
        }


        document.addEventListener("DOMContentLoaded", () => {
            const form = document.getElementById("rfqForm");
        form.addEventListener("submit", function (e) {
            e.preventDefault();
        prepareAndSubmitForm();
            });
        });
    