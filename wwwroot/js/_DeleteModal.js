let deleteId = null;

function openDeleteModal(id, name, stock) {
  let message = "";
  if (stock > 0) {
    message = `${name} ürününün stokta ${stock} adet var. Silmek istediğinizden emin misiniz?`;
  } else {
    message = `${name} ürününü silmek istediğinizden emin misiniz?`;
  }

  document.getElementById("deleteMessage").innerText = message;
  deleteId = id;

  var myModal = new bootstrap.Modal(document.getElementById("deleteModal"));
  myModal.show();
}

function confirmDelete() {
  let confirmMessage = "Bu ürünü silmek istediğinizden emin misiniz?";
  document.getElementById("confirmDeleteMessage").innerText = confirmMessage;

  var myModal = new bootstrap.Modal(
    document.getElementById("confirmDeleteModal")
  );
  myModal.show();

  document.getElementById("deleteForm").action =
    "/Product/DeleteConfirmed/" + deleteId;
}

$("#productTable").DataTable({
  language: {
    url: "//cdn.datatables.net/plug-ins/1.13.6/i18n/Turkish.json",
  },
  dom:
    "<'row my-3'<'col-sm-12 col-md-6'B><'col-sm-12 col-md-6'f>>" +
    "rt" +
    "<'row mt-3'<'col-sm-12 col-md-5'l><'col-sm-12 col-md-7'p>>",
  buttons: [
    {
      extend: "copyHtml5",
      exportOptions: {
        columns: ":not(:last-child)",
        format: {
          body: function (data, row, column, node) {
            if (column === 4 || column === 5) {
              return node
                .querySelector("i")
                .classList.contains("fa-circle-check")
                ? "+"
                : "-";
            }
            return data;
          },
        },
      },
    },
    {
      extend: "excelHtml5",
      exportOptions: {
        columns: ":not(:last-child)",
        format: {
          body: function (data, row, column, node) {
            if (column === 4 || column === 5) {
              return node
                .querySelector("i")
                .classList.contains("fa-circle-check")
                ? "+"
                : "-";
            }
            return data;
          },
        },
      },
    },
    {
      extend: "pdfHtml5",
      exportOptions: {
        columns: ":not(:last-child)",
        format: {
          body: function (data, row, column, node) {
            if (column === 4 || column === 5) {
              return node
                .querySelector("i")
                .classList.contains("fa-circle-check")
                ? "+"
                : "-";
            }
            return data;
          },
        },
      },
    },
    {
      extend: "print",
      exportOptions: {
        columns: ":not(:last-child)",
        format: {
          body: function (data, row, column, node) {
            if (column === 4 || column === 5) {
              return node
                .querySelector("i")
                .classList.contains("fa-circle-check")
                ? "+"
                : "-";
            }
            return data;
          },
        },
      },
    },
    "colvis",
  ],
  columnDefs: [
    {
      targets: -1,
      orderable: false,
      searchable: false,
    },
  ],
});
