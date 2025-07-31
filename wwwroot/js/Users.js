$(document).ready(function () {
    FetchUser();
});

$("#modalbtn").click(function () {
    $("#exampleModal").modal('show');
    $("#submitform")[0].reset();
    $("#id_div").hide();
    $("#savebtn").show();
    $("#updbtn").hide();
});

$("#savebtn").click(function () {
    var obj = $("#submitform").serialize();

    console.log(obj);
    $.ajax({
        url: '/AddUser/AddUsers',
        method: 'Post',
        dataType: 'Json',
        data: obj,
        contentType: 'application/x-www-form-urlencoded;charset=utf-8',
        success: function () {
         

            FetchUser();
            $('#exampleModal').removeClass('show');
            $('#exampleModal').css('display', 'none');
            $('.modal-backdrop').remove();
            $('body').removeClass('modal-open');
            $("#submitform")[0].reset();

            toastr.success('User added successfully');

        },
        error: function () {
            toastr.error('not loaded');
        }
    });
});

function FetchUser() {
    $.ajax({
        url: '/AddUser/FetchUsers',
        type: 'Get',
        dataType: 'Json',

        contentType: 'application/json;charset=utf-8',
        success: function (response) {
            var obj = '';
            $.each(response, function (index, item) {
                obj += '<tr>';
                obj += `<td>${item.userId}</td>`;
                obj += `<td>${item.fullName}</td>`;
                obj += `<td>${item.email}</td>`;
                obj += `<td>${item.passwordHash}</td>`;
                obj += `<td>${item.role}</td>`;

                obj += `<td>
                <a href="#" class="text-primary me-3 fw-bold f  s-5" onclick="EditUserDetails(${item.userId})" title="Edit">
                    <i class="bi bi-pencil-square"></i>
                </a>
                <a href="#" class="fw-bold fs-5" onclick="DelUser(${item.userId})" title="Delete">
                    <i class="bi bi-trash" style="color: blue;"></i>
                </a>
            </td>`;

                obj += '</tr>';
            });
            $("#mydata").html(obj);
        },
        error: function () {
            toastr.error('not loaded');

        }

    });
}
function DelUser(uid) {
    if (confirm('Are you sure')) {
        $.ajax({
            url: '/AddUser/DeleteUser?id=' + uid,
            dataType: 'Json',

            success: function () {
                toastr.success('User deleted Successfully');
                FetchUser()

            },
            error: function () {
                toastr.error('not loaded');
            }

        });
    }
    else {
        toastr.error('Not an Issue');
    }

}



$("#txt").keyup(function () {
    var data = $("#txt").val();
    $.ajax({
        url: '/AddUser/SearchUser?mydata=' + data,
        dataType: 'json',
        type: 'Get',
        contentType: 'application/json;charset=utf-8',
        success: function (response) {
            var obj = '';
            $.each(response, function (index, item) {
                obj += '<tr>';
                obj += `<td>${item.userId}</td>`;
                obj += `<td>${item.fullName}</td>`;
                obj += `<td>${item.email}</td>`;
                obj += `<td>${item.passwordHash}</td>`;
                obj += `<td>${item.role}</td>`;
                obj += `<td>
                 <a href="#" class="me-3 fw-bold fs-5" onclick="EditUserDetails(${item.userId})" title="Edit">
                    <i class="bi bi-pencil-square" style="color: black;"></i>
                </a>
                <a href="#" class="fw-bold fs-5" onclick="DelUser(${item.userId})" title="Delete">
                    <i class="bi bi-trash" style="color: black;"></i>
                </a>
            </td>`;
                obj += '</tr>';
            });

            $("#mydata").html(obj);
        },
        error: function () {

        }
    });
});

function EditUserDetails(id) {
    $.ajax({
        url: '/AddUser/EditUsers?id=' + id,
        type: 'Get',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (res) {
            $("#exampleModal").modal('show');
            $("#savebtn").hide();
            $("#updbtn").show();
            $("#id_div").show();

            $("#userid").val(res.userId);
            $("#fullname").val(res.fullName);
            $("#email").val(res.email);
            $("#password").val(res.passwordHash);
            $("#role").val(res.role);

        },
        error: function () {
            alert('not');
        }
    });
}

$("#updbtn").click(function () {
    var obj = $("#submitform").serialize();

    $.ajax({
        url: '/AddUser/UpdateUsers',
        type: 'Post',
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded;charset=utf-8',
        data: obj,
        success: function () {
            toastr.success(' User Updated Successfully');
            $('#exampleModal').removeClass('show');
            $('#exampleModal').css('display', 'none');
            $('.modal-backdrop').remove();
            $('body').removeClass('modal-open');


            FetchUser()
        },
        error: function () {
            toastr.error('Not Updated');
        }
    });
});