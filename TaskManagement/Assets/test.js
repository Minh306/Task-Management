

//  thêm nhiệm vụ mới
var btn = document.getElementById("mybtn");

var modal = document.getElementById("myModalCreateMission");

var btnCancel = document.getElementById("btnCancel");

var btnCreate = document.getElementById("btnCreate");

window.onload = function() {
  btn.onclick = function () {
    modal.style.display = "block";
  };
  btnCancel.onclick = function () {
    modal.style.display = "none";
  };
  btnCreate.onclick = function () {
    modal.style.display = "none";
  };
}
// datatable
window.onload = function(){
  $(document).ready(function () {
    $('#example').DataTable({
        responsive: true,
        dom: '<"toolbar">frtip',
        "bAutoWidth": false,
        "bInfo": false
    });
  });
}


// chỉnh sửa nhiệm vụ mục tiêu
var edit1 = document.getElementsByClassName("edit1");

var modalEditMission = document.getElementById("myModalEditMission");

var btnCancel1 = document.getElementById("btnCancel1");

var btnCreate1 = document.getElementById("btnCreate1");
window.onload = function(){
for (var i = 0; i < edit1.length; i++) {
  edit1[i].onclick = function (e) {
    modalEditMission.style.display = "block";

    e.preventDefault();
    $("#inputName1").val(
      $(this).closest("tr").find("td:nth-child(1)").text().trim(" ")
    );
    $("#inputDescription1").val(
      $(this).closest("tr").find("td:nth-child(8)").text().trim(" ")
    );
  };
}
}
window.onload = function() {
  btnCancel1.onclick = function () {
    modalEditMission.style.display = "none";
  };
  btnCreate1.onclick = function () {
    modalEditMission.style.display = "none";
  };
}

// Kết thúc chỉnh sửa nhiệm vụ mục tiêu

// chỉnh sửa công việc
var edit2 = document.getElementsByClassName("edit2");

var modalEditWork = document.getElementById("myModalEditWork");

var btnCancel2 = document.getElementById("btnCancel2");

var btnCreate2 = document.getElementById("btnCreate2");

for (var i = 0; i < edit2.length; i++) {
  edit2[i].onclick = function (e) {
    modalEditWork.style.display = "block";

    e.preventDefault();
    $("#inputName2").val(
      $(this).closest("tr").find("td:nth-child(1)").text().trim(" ")
    );
    $("#inputDescription2").val(
      $(this).closest("tr").find("td:nth-child(8)").text().trim(" ")
    );
  };
}
window.onload = function() {
btnCancel2.onclick = function () {
  modalEditWork.style.display = "none";
};
btnCreate2.onclick = function () {
  modalEditWork.style.display = "none";
};
}

$("#reservation2").daterangepicker({
  locale: {
    format: "DD/MM/YYYY",
  },
});
// Kết thúc chỉnh sửa công việc

// var delete1 = document.getElementsByClassName("delete1");
// var delete2 = document.getElementsByClassName("delete2");

// for (var i = 0; i < delete1.length; i++) {
//   delete1[i].onclick = function () {
//     var result = confirm("Bạn có chắc muốn xoá nhiệm vụ/ mục tiêu này không?");

//     if (result) {
//       alert("Xoá thành công");
//     }
//   };
// }
// for (var i = 0; i < delete2.length; i++) {
//   delete2[i].onclick = function () {
//     var result = confirm("Bạn có chắc muốn xoá công việc này không?");

//     if (result) {
//       alert("Xoá thành công");
//     }
//   };
// }
// Kết thúc chỉnh sửa công việc

// Nhấn màn hình tự thoát thêm nhiệm vụ, chỉnh sửa nhiệm vụ, chỉnh sửa công việc
window.onclick = function (event) {
  if (event.target == modal) {
    modal.style.display = "none";
  } else if (event.target == modalEditMission) {
    modalEditMission.style.display = "none";
  } else if (event.target == modalEditWork) {
    modalEditWork.style.display = "none";
  }
};
// Kết thúc Nhấn màn hình tự thoát thêm nhiệm vụ, chỉnh sửa nhiệm vụ, chỉnh sửa công việc


var badgeWidthStatus = document.getElementsByClassName("status");
var hideButton = document.getElementsByClassName("hide-button");
// // const valueStatus = badgeWidthStatus.innerHTML.normalize();
// const valueDelay = "Trễ tiến độ".normalize();

for (var i = 0; i < badgeWidthStatus.length; i++) {
  if (
    badgeWidthStatus[i].innerHTML.normalize() === "Trễ tiến độ" ||
    badgeWidthStatus[i].innerHTML.normalize() === "Đang thực hiện"
  ) {
    hideButton[i].style.display = "none";
  }
}

var fillterStatus = document.getElementById("fillterStatus");
var dropdown_content_test = document.getElementById("dropdown-content-test");
var status_tablehead = document.getElementsByClassName("status-tablehead");
var hello_demo = document.getElementById("hello-demo");
var hello_demo1 = document.getElementById("hello-demo1");

window.onload = function() {
  if(fillterStatus){
fillterStatus.onclick = function () {
  if (fillterStatus.className.match(/(?:^|\s)right fas fa-angle-left(?!\S)/)) {
    fillterStatus.className = "right fas fa-angle-down";
    dropdown_content_test.style.display = "block";
  } else if (
    fillterStatus.className.match(/(?:^|\s)right fas fa-angle-down(?!\S)/)
  ) {
    fillterStatus.className = "right fas fa-angle-left";
    dropdown_content_test.style.display = "none";
  }
};
}
}