$(document).ready(function () {
    $('#categoriesTable').DataTable({
        dom:
            "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-5'i><'col-sm-7'p>>",
        "order": [[6, "desc"]],
        buttons: [
            {
                text: 'Ekle',
                attr: {
                    id: "btnAdd",
                },
                className: 'btn btn-success',
                action: function (e, dt, node, config) {

                }
            },
            {
                text: 'Yenile',
                className: 'btn btn-warning',
                action: function (e, dt, node, config) {

                    $.ajax({
                        type: 'GET',
                        url: '/Admin/Category/GetAllCategories/',
                        contentType: "application/json",       // json formatında çalışıcağımızı belirtmiş olduk.
                        beforeSend: function () {
                            $('#categoriesTable').hide();      // tabloyu gizledik
                            $('.spinner-border').show();       // yükleniyor.. simgesini gösterdik
                        },
                        success: function (data) {           // data değişkeni CategoryListDto dönüyor
                            const categoryListDto = jQuery.parseJSON(data);     // Gelen veriyi jsondan parse ediyoruz.
                            console.log(categoryListDto);
                            if (categoryListDto.ResultStatus === 0) {
                                let tableBody = "";
                                $.each(categoryListDto.Categories.$values,
                                    function (index, category) {
                                        tableBody += `
                                                          <tr name="${category.Id}">
                                    
                                    <td>${category.Id}</td>
                                    <td>${category.Name}</td>
                                    <td>${category.Description}</td>
                                    <td>${category.IsActive ? "Evet" : "Hayır"}</td>
                                    <td>${category.IsDeleted ? "Evet" : "Hayır"}}</td>
                                    <td>${category.Note}</td>
                                    <td>${convertToShortDate(category.CreatedDate)}</td>
                                    <td>${category.CreatedByName}</td>
                                    <td>${convertToShortDate(category.ModifiedDate)}</td>
                                    <td>${category.ModifiedByName}</td>
                                    <td>
                                <button class="btn btn-primary btn-sm btn-update" data-id="${category.Id}" ><span class="fas fa-edit"></span> </button>
                                <button class="btn btn-danger btn-sm btn-delete" data-id="${category.Id}"><span class="fas fa-minus-circle"></span> </button>
                                    </td>
                                            </tr>`;
                                    });
                                $('#categoriesTable > tbody').replaceWith(tableBody);   // idsi categoriesTable olan tablonun bodysine erişip yeni tablomuzşa yer değiştiriyoruz.
                                $('.spinner-border').hide();
                                $('#categoriesTable').fadeIn(1400);
                            } else {
                                toastr.error(`${categoryListDto.Message}`, 'İşlem Başarısız!');
                            }
                        },
                        error: function (err) {
                            console.log(err);
                            $('.spinner-border').hide();
                            $('#categoriesTable').fadeIn(1000);
                            toastr.error(`${err.responseText}`, 'Hata!');
                        }
                    });
                }
            }
        ],
        language: {
            "sDecimal": ",",
            "sEmptyTable": "Tabloda herhangi bir veri mevcut değil",
            "sInfo": "_TOTAL_ kayıttan _START_ - _END_ arasındaki kayıtlar gösteriliyor",
            "sInfoEmpty": "Kayıt yok",
            "sInfoFiltered": "(_MAX_ kayıt içerisinden bulunan)",
            "sInfoPostFix": "",
            "sInfoThousands": ".",
            "sLengthMenu": "Sayfada _MENU_ kayıt göster",
            "sLoadingRecords": "Yükleniyor...",
            "sProcessing": "İşleniyor...",
            "sSearch": "Ara:",
            "sZeroRecords": "Eşleşen kayıt bulunamadı",
            "oPaginate": {
                "sFirst": "İlk",
                "sLast": "Son",
                "sNext": "Sonraki",
                "sPrevious": "Önceki"
            },
            "oAria": {
                "sSortAscending": ": artan sütun sıralamasını aktifleştir",
                "sSortDescending": ": azalan sütun sıralamasını aktifleştir"
            },
            "select": {
                "rows": {
                    "_": "%d kayıt seçildi",
                    "0": "",
                    "1": "1 kayıt seçildi"
                }
            }
        }
    });
    // DataTables end here 
    // Ajax GET / Getting the _CategoryAddPartial as Modal Form starts from here. 

    $(function () {
        const url = '/Admin/Category/Add/';                      // hangi actiona gideceğini url değişkenine atıyoruz.
        const placeHolderDiv = $('#modalPlaceHolder');           // id si modalPlaceHolder olan divi placeHolderDiv değişkenine atadık.Burdaki div içersine formumuzu eklemek isritoruz.
        $('#btnAdd').click(function () {                         // id si btnAdd olan butona tıklandığında
            $.get(url).done(function (data) {                     // burdaki get işlemi add actionuna gidip bizim partialviewımızı alıp getiriyor olacak.
                placeHolderDiv.html(data);                       // bu işlem bittiğinde de yeni bir fonksiyon oluşturuyoruz ve bunun içine data diye bir değer gelicek.data: categoryAddPartialViewimiz
                placeHolderDiv.find(".modal").modal('show');     // bu partialımızı id si modalPlaceHolder olan divin içindeki htmle ekliyoruz.
                // yukarıdaki satırda da modal olarak gelmesi için onu jquery ile işaretliyoruz.Sen bir modal olarak açıl diyoruz.
                // partialview içindeki modal classını bul ve modal yap show ile de göster.
            });
        });

        //   Ajax GET / Getting the _CategoryAddPartial as Modal Form ends here. 
        //   Ajax POST / Posting the FormData as CategoryAddDto starts from here. 

        placeHolderDiv.on('click',                                              //  burdaki div üzerinde click eventi tetiklendiğinde çalışıcak işlemleri yazmamızı sağlıyor.
            '#btnSave',                                                          // _CategoryAddPartialda bulunan btnSave idli button (kısaca burda kaydet butonuna tıklanıldığında denmek isteniyor.)
            function (event) {
                event.preventDefault();                                          // her ne kadarda jquery kodu eklemiş olsakta butona tıkladığımızda sayfanın yenilenmesini istemiyoruz o yüzden bu kodu yazdık.Örn submit butonu yenilenir.
                const form = $('#form-category-add');                            // _CategoryAddPartial içindeki gönderceğimiz formu seçtik.
                const actionUrl = form.attr('action');                           // formumuz içindeki action urli okumamız gerek.asp-action="Add" kısmındaki Add bize url üreticek.Yani bu form hangi urle post edilmeli diyoruz.
                const dataToSend = form.serialize();                             // form içindeki veriyi formu serileştirerek alıyoruz.Aslında form içindeki veriyi CategoryAddDto olarak dönüştürmüş olduk.
                $.post(actionUrl, dataToSend).done(function (data) {             // formdan gerekl verileri de aldık ve gerekli urle post edebiliriz.1.parametre post işlemini hangi urle yapmalıyım 2. neyi post edicez.
                    console.log(data);
                    const categoryAddAjaxModel = jQuery.parseJSON(data);         // return işleminde gelen modelimizi(data) Js içersinde okuyabilmek ve kullanbilmek için Jsondan parse etmemiz gerek.
                    console.log(categoryAddAjaxModel);                           // Artık elimizde c# tarafında old gibi CategoryAddAjaxViewModel olmuş oldu.
                    const newFormBody = $('.modal-body', categoryAddAjaxModel.CategoryAddPartial);        //Bizlere gelen bu partialviewı form modal içersine basmamız gerek.Modelstate kontrolü yaptıktan sonra onunla ilgili veriler işlendikten sonra aslında bize yeni bir partialview dönüyor.
                    placeHolderDiv.find('.modal-body').replaceWith(newFormBody);                          // Yukarıda yeni modal-bodyi aldık burda da eskisiyle yer değiştirdik.Bu sayede örn kategori adı girmeden gönderilmeye çalışılırsa hata alıyor olacağız.
                    const isValid = newFormBody.find('[name="IsValid"]').val() === 'True';
                    if (isValid) {                                                                  // Model durumumuz doğruysa post işlemi gerçekleşti o zaman modal formu kapat,tabloya efektli şekilde yeni kaydı ekle daha sonra da toastr mesajını çıkart.
                        placeHolderDiv.find('.modal').modal('hide');                                // modal div kısmını bul bunu modal yap ve gizle
                        const newTableRow = `
                                <tr name="${categoryAddAjaxModel.CategoryDto.Category.Id}">
                                                    <td>${categoryAddAjaxModel.CategoryDto.Category.Id}</td>
                                                    <td>${categoryAddAjaxModel.CategoryDto.Category.Name}</td>
                                                    <td>${categoryAddAjaxModel.CategoryDto.Category.Description}</td>
                                                    <td>${categoryAddAjaxModel.CategoryDto.Category.IsActive ? "Evet" : "Hayır"}</td>
                                                    <td>${categoryAddAjaxModel.CategoryDto.Category.IsDeleted ? "Evet" : "Hayır"}</td>
                                                    <td>${categoryAddAjaxModel.CategoryDto.Category.Note}</td>
                                                    <td>${convertToShortDate(categoryAddAjaxModel.CategoryDto.Category.CreatedDate)}</td>
                                                    <td>${categoryAddAjaxModel.CategoryDto.Category.CreatedByName}</td>
                                                    <td>${convertToShortDate(categoryAddAjaxModel.CategoryDto.Category.ModifiedDate)}</td>
                                                    <td>${categoryAddAjaxModel.CategoryDto.Category.ModifiedByName}</td>
                                                  
                                                    <td>
                                                        <button class="btn btn-primary btn-sm btn-update" data-id="${categoryAddAjaxModel.CategoryDto.Category.Id}"><span class="fas fa-edit"></span></button>
                                                        <button class="btn btn-danger btn-sm btn-delete" data-id="${categoryAddAjaxModel.CategoryDto.Category.Id}"><span class="fas fa-minus-circle"></span></button>
                                                    </td>
                                                </tr>`;
                        const newTableRowObject = $(newTableRow);                 // Bu tablerow aslında string bunu tabloya ekleyemeyiz o yüzden jquery objesine dönüştürdük.
                        newTableRowObject.hide();
                        $('#categoriesTable').append(newTableRowObject);          // Tablonun sonuna bizim ekleyeceğimiz html bigisini(yeni eklenen değeri) ekliyor
                        newTableRowObject.fadeIn(3500);                           // Tablonun efektif bir şekilde gelmesini sağladı.
                        toastr.success(`${categoryAddAjaxModel.CategoryDto.Message}`, 'Başarılı İşlem!');           // Toastr mesajımızı gösterdik.
                    }
                    // Isvalid true gelmezse hata mesajını bir toastr içinde göstericez.

                    else {
                        let summaryText = "";
                        $('#validation-summary > ul > li').each(function () {         // Modelstate hatalı olarak gönderdiğimizde orda bir div açılıyor ve içinde bir ul ve onun içinde de li ler oluyor.
                            let text = $(this).text();                                // bu li lerde foreachle dönüp her bi linin textini text değişkenine atıyoruz.
                            summaryText = `*${text}\n`;                               // daha sonra bu textleri summarytexte birleştiriyoruz ve toastra veriyoruz.
                        });
                        toastr.warning(summaryText);
                    }
                });
            });
    });

    // Delete ajax post işlemleri

    $(document).on('click',
        '.btn-delete',
        function (event) {
            event.preventDefault();
            const id = $(this).attr('data-id');
            const tableRow = $(`[name="${id}"]`);
            const categoryName = tableRow.find('td:eq(1)').text();
            // Sweet Alert için kodlarımız
            Swal.fire({
                title: 'Silmek istediğinize emin misiniz?',
                text: `${categoryName} adlı kategori silinicektir!`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Evet, silmek istiyorum.',
                cancelButtonText: 'Hayır, silmek istemiyorum.'
            }).then((result) => {
                if (result.isConfirmed) {       // Evete tıklandığında
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        data: { categoryId: id },
                        url: '/Admin/Category/Delete/',
                        success: function (data) {
                            const categoryDto = jQuery.parseJSON(data);
                            if (categoryDto.ResultStatus === 0) {          // Silindi ise
                                Swal.fire(
                                    'Silindi!',
                                    `${categoryDto.Category.Name} adlı kategori başarıyla silinmiştir.`,
                                    'success'
                                );

                                tableRow.fadeOut(3500);                 // Bu tablerowun efektif bir şekilde gizlenmesini sağladık.
                            } else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Başarısız İşlem!',
                                    text: `${categoryDto.Message}`,
                                });
                            }
                        },
                        error: function (err) {
                            console.log(err);
                            toastr.error(`${err.responseText}`, "Hata!")
                        }
                    });
                }
            });
        });
    $(function () {
        const url = '/Admin/Category/Update/';
        const placeHolderDiv = $('#modalPlaceHolder');
        $(document).on('click',
            '.btn-update',
            function (event) {
                event.preventDefault();
                const id = $(this).attr('data-id');
                $.get(url, { categoryId: id }).done(function (data) {
                    placeHolderDiv.html(data);
                    placeHolderDiv.find('.modal').modal('show');
                }).fail(function () {
                    toastr.error("Bir hata oluştu.");
                });
            });
        // Update post işlemleri

        placeHolderDiv.on('click',
            '#btnUpdate',
            function (event) {
                event.preventDefault();

                const form = $('#form-category-update');
                const actionUrl = form.attr('action');
                const dataToSend = form.serialize();
                $.post(actionUrl, dataToSend).done(function (data) {
                    const categoryUpdateAjaxModel = jQuery.parseJSON(data);
                    console.log(categoryUpdateAjaxModel);
                    const newFormBody = $('.modal-body', categoryUpdateAjaxModel.CategoryUpdatePartial);
                    placeHolderDiv.find('.modal-body').replaceWith(newFormBody);
                    const isValid = newFormBody.find('[name="IsValid"]').val() === 'True';
                    if (isValid) {
                        placeHolderDiv.find('.modal').modal('hide');
                        const newTableRow = `
                                            <tr name="${categoryUpdateAjaxModel.CategoryDto.Category.Id}">
                                                    <td>${categoryUpdateAjaxModel.CategoryDto.Category.Id}</td>
                                                    <td>${categoryUpdateAjaxModel.CategoryDto.Category.Name}</td>
                                                    <td>${categoryUpdateAjaxModel.CategoryDto.Category.Description}</td>
                                                    <td>${categoryUpdateAjaxModel.CategoryDto.Category.IsActive ? "Evet" : "Hayır"}</td>
                                                    <td>${categoryUpdateAjaxModel.CategoryDto.Category.IsDeleted ? "Evet" : "Hayır"}</td>
                                                    <td>${categoryUpdateAjaxModel.CategoryDto.Category.Note}</td>
                                                    <td>${convertToShortDate(categoryUpdateAjaxModel.CategoryDto.Category.CreatedDate)}</td>
                                                    <td>${categoryUpdateAjaxModel.CategoryDto.Category.CreatedByName}</td>
                                                    <td>${convertToShortDate(categoryUpdateAjaxModel.CategoryDto.Category.ModifiedDate)}</td>
                                                    <td>${categoryUpdateAjaxModel.CategoryDto.Category.ModifiedByName}</td>
                                                    <td>
                                                        <button class="btn btn-primary btn-sm btn-update" data-id="${categoryUpdateAjaxModel.CategoryDto.Category.Id}"><span class="fas fa-edit"></span></button>
                                                        <button class="btn btn-danger btn-sm btn-delete" data-id="${categoryUpdateAjaxModel.CategoryDto.Category.Id}"><span class="fas fa-minus-circle"></span></button>
                                                    </td>
                                                </tr>`;
                        const newTableRowObject = $(newTableRow);
                        const categoryTableRow = $(`[name="${categoryUpdateAjaxModel.CategoryDto.Category.Id}"]`);
                        newTableRowObject.hide();
                        categoryTableRow.replaceWith(newTableRowObject);
                        newTableRowObject.fadeIn(3500);
                        toastr.success(`${categoryUpdateAjaxModel.CategoryDto.Message}`, "Başarılı İşlem!");
                    } else {
                        let summaryText = "";
                        $('#validation-summary > ul > li').each(function () {
                            let text = $(this).text();
                            summaryText = `*${text}\n`;
                        });
                        toastr.warning(summaryText);
                    }
                }).fail(function (response) {
                    console.log(response);
                });
            });


    });
});