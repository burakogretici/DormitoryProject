var kvs = function () {
    return {
        hideMask: function (block) {
            if (block) {
                $(block).unblock();
            }
            else {
                $.unblockUI();
            }
        },
        showMask: function (block, msg) {
            if (block) {
                $(block).block({
                    message: '<i class="fa fa-2x fa-spinner fa-spin"></i>' + (msg != undefined ? msg : ""),
                    overlayCSS: {
                        backgroundColor: '#fff',
                        opacity: 0.8,
                        cursor: 'wait',
                        'box-shadow': '0 0 0 1px #ddd'
                    },
                    css: {
                        border: 0,
                        padding: 0,
                        backgroundColor: 'none'
                    }
                });
            }
            else {
                $.blockUI({
                    message: '<i class="fa fa-2x fa-spinner fa-spin"></i>' + (msg != undefined ? msg : ""),
                    overlayCSS: {
                        backgroundColor: '#fff',
                        opacity: 0.8,
                        cursor: 'wait',
                        'box-shadow': '0 0 0 1px #ddd'
                    },
                    css: {
                        border: 0,
                        padding: 0,
                        backgroundColor: 'none'
                    }
                });
            }
        },
        gotoUrl: function (url) {
            kvs.showMask("body", "Yönlendiriliyorsunuz. Lütfen bekleyiniz...");
            window.location.href = url;
        },
        showModal: function (type, title, body, fn, fnClose) {
            var bgcss = type == "danger" ? "bg-danger" : "bg-primary";
            var modal = $("<div>", { class: "modal fade", tabindex: "-1" }).append(
                $("<div>", { class: "modal-dialog" }).append(
                    $("<div>", { class: "modal-content" }).append(
                        $("<div>", { class: "modal-header " + bgcss }).append(
                            $("<h6>", { class: "modal-title text-white", html: title }),
                            $("<button>", { type: "button", class: "close text-white", "data-dismiss": "modal", html: "&times;" }),
                        ),
                        $("<div>", { class: "modal-body" }).append(
                            body
                        ),
                        $("<div>", { class: "modal-footer" }).append(
                            $("<button>", { type: "button", class: "btn btn-secondary", "data-dismiss": "modal", html: "Kapat" }),
                            $("<button>", { type: "button", class: "btn ok-btn " + (type == "danger" ? "btn-danger" : "btn-warning"), html: "Kaydet" })
                        )
                    )
                )
            );
            if (!title) {
                modal.find(".modal-header").remove();
            }
            if (!fn) {
                modal.find(".ok-btn").remove();
            }

            modal.appendTo("body");

            if (fn && typeof fn == "function") {
                modal.find(".ok-btn").click(fn);
            }

            modal.on("hidden.bs.modal", function (e) {
                if (e.target === this) {
                    modal.remove();
                    if ($(".modal.show").length) {
                        $("body").addClass("modal-open");
                    }
                    if (fnClose && typeof fnClose == "function") {
                        fnClose();
                    }
                }
            });
            modal.modal({ backdrop: 'static' });
            var zindex = 0;
            $(".modal").each(function (i, e) {
                var _zindex = parseInt($(e).css("z-index"));
                if (_zindex > zindex) {
                    zindex = _zindex;
                }
            });
            //var zindex = $(".modal:last").css("z-index");
            if (zindex > 0) {
                var z = parseInt(zindex);
                $(".modal-backdrop:last").css("z-index", z + 1);
                modal.css("z-index", z + 2);
            }

            return modal;
        },
        alert: function (type, title, msg, fnClose) {
            kvs.showModal(type, title, msg, undefined, fnClose);
        },
        confirm: function (title, msg, fn, css) {
            if (!css) {
                css = "success";
            }
            var confirmModal = kvs.showModal(css, title, msg, function () {
                fn();
                confirmModal.modal("hide");
            });
            confirmModal.find(".btn-link").html("İptal");
            confirmModal.find(".ok-btn").html("Tamam");
        },
        openLinkModal: function (title, url, data, fn, fnLoad, fnClose) {
            var modal = kvs.showModal("success", title, "<div class='inner-content'></div>", fn, fnClose);
            modal.find(".ok-btn").html("<i class='fas fa-save'></i> Kaydet");
            modal.find(".modal-dialog").addClass("modal-lg");

            if (data) {
                if ($.isPlainObject(data)) {
                    data["modal"] = true;
                }
                else if (typeof data == "string") {
                    if (kvs.isEmpty(data)) {
                        data = "modal=true"
                    }
                    else if (data.indexOf("?") >= 0) {
                        data += "&modal=true";
                    }
                    else {
                        data += "?modal=true";
                    }
                }
            }

            kvs.getJx(url, modal.find(".modal-content"), data, function (result) {
                modal.find(".modal-body").html(result);
                if (fnLoad && typeof fnLoad == "function") {
                    fnLoad();
                }
            }, function () {
                modal.find(".modal-body").append(
                    $("<div>", { class: "alert bg-danger text-white", html: "Veriler yüklenemedi. Lütfen daha sonra tekrar deneyiniz" })
                );
            });
            return modal;
        },
        callJx: function (url, mask, data, done, fail) {
            kvs.showMask(mask);
            
            var jx = $.ajax({
                type: 'POST',
                url: url,
                data: data,
                cache: false,
                success: function (result) {
                    kvs.hideMask(mask);

                    if (result.redirect) {
                        kvs.gotoUrl(result.redirect);
                        return;
                    }

                    if (result.hasError !== undefined) {
                        if (result.hasError) {
                            if (result.errors && result.errors.length) {
                                kvs.showModal("danger", "Hata", kvs.resultError(result));
                            }
                            return;
                        }
                    }

                    if (done && typeof done === 'function') {
                        done.call(undefined, result);
                    }
                    kvs.ajaxDone();
                },
                error: function (xhr, status, error) {
                    
                    kvs.hideMask(mask);
                    if (fail && typeof fail === 'function') {
                        fail.call(undefined);
                    }
                    else {
                        if (status != "abort") {
                            kvs.showModal("danger", "Hata", "Beklenmedik bir hata oluştu");
                        }
                    }
                }
            });
            return jx;
        },
        getJx: function (url, mask, data, done, fail) {
            kvs.showMask(mask);

            var jx = $.ajax({
                type: 'GET',
                url: url,
                data: data,
                cache: false,
                success: function (result) {
                    kvs.hideMask(mask);

                    if (result && typeof result === 'object') {
                        if (result.redirect) {
                            kvs.gotoUrl(result.redirect);
                            return;
                        }

                        if (result.hasError !== undefined) {
                            if (result.hasError) {
                                if (result.errors && result.errors.length) {
                                    kvs.showModal("danger", "Hata", kvs.resultError(result));
                                }
                                return;
                            }
                        }
                    }

                    if (done && typeof done === 'function') {
                        done.call(undefined, result);
                    }
                    kvs.ajaxDone();
                },
                error: function (xhr, status, error) {
                    
                    kvs.hideMask(mask);
                    if (fail && typeof fail === 'function') {
                        fail.call(undefined);
                    }
                    else {
                        kvs.showModal("danger", "Hata", "Beklenmedik bir hata oluştu");
                    }
                }
            });

            return jx;
        },
        postJx: function (url, mask, form, done, fail) {
            kvs.showMask(mask);

            var data = new window.FormData($(form).get(0));

            var jx = $.ajax({
                type: 'POST',
                method: 'POST',
                data: data,
                cache: false,
                contentType: false,
                processData: false,
                url: url,
                success: function (result) {
                    kvs.hideMask(mask);

                    if (result.redirect) {
                        kvs.gotoUrl(result.redirect);
                        return;
                    }

                    if (result.hasError !== undefined) {
                        if (result.hasError) {
                            if (result.errors && result.errors.length) {
                                kvs.showModal("danger", "Hata", kvs.resultError(result));
                            }
                            return;
                        }
                    }

                    if (done && typeof done === 'function') {
                        done.call(undefined, result);
                    }
                    kvs.ajaxDone();
                },
                error: function (xhr, status, error) {
                    
                    kvs.hideMask(mask);
                    if (fail && typeof fail === 'function') {
                        fail.call(undefined);
                    }
                    else {
                        kvs.showModal("danger", "Hata", "Beklenmedik bir hata oluştu");
                    }
                }
            });
            return jx;
        },
        resultError: function (result) {
            
            if (!result.errors) {
                return "Beklenmedik bir hata oluştu";
            }
            return result.errors.join("<br />");
        },
        value: function (obj, prop) {
            prop = (prop + "").toLowerCase();
            var props = prop.split(".");
            for (var i = 0; i < props.length; i++) {
                var found = false;
                for (var p in obj) {
                    if (obj.hasOwnProperty(p) && props[i] == (p + "").toLowerCase()) {
                        found = true;
                        if (i == props.length - 1) {
                            return obj[p];
                        }
                        else {
                            obj = obj[p];
                            break;
                        }
                    }
                }
                if (!found) {
                    break;
                }
            }
        },
        fitHeight: function (el) {
            var element = $(el);
            element.css("max-height", "").css("overflow-y", "");

            var minus = 0;
            //element.parent().find("> *:visible:not(.max-height)").each(function () { minus += $(this).height(); });
            var elements = element.find("> *:visible").hide();
            var height = element.height() - minus;
            elements.show();
            if (height > 0) {
                element.css("max-height", height + "px").css("overflow-y", "auto");
            }
        },
        matchHeight: function (el) {
            var element = $(el);
            var target = $(element.attr("data-target"));
            var maxHeight = 0;
            element.each(function () {
                if (maxHeight < $(this).height()) {
                    maxHeight = $(this).height();
                }
            });
            target.each(function () {
                if (maxHeight < $(this).height()) {
                    maxHeight = $(this).height();
                }
            });
            if (maxHeight > 100) {
                target.css("min-height", maxHeight);
                element.css("min-height", maxHeight);
            }
        },
        isEmpty: function (input) {
            return input == undefined || input == null || input == "" || (typeof (input) == "object" && Object.keys(input).length == 0);
        },
        format: function () {
            var formatted = arguments[0];
            for (var i = 1; i < arguments.length; i++) {
                var regexp = new RegExp('\\{' + (i - 1) + '\\}', 'gi');
                formatted = formatted.replace(regexp, arguments[i]);
            }
            return formatted;
        },
        formatNumber: function (number, decPlaces, decSep, thouSep) {
            decPlaces = isNaN(decPlaces = Math.abs(decPlaces)) ? 2 : decPlaces;
            decSep = typeof decSep === "undefined" ? "," : decSep;
            thouSep = typeof thouSep === "undefined" ? "." : thouSep;
            var sign = number < 0 ? "-" : "";
            var i = String(parseInt(number = Math.abs(Number(number) || 0).toFixed(decPlaces)));
            var j = (j = i.length) > 3 ? j % 3 : 0;

            return sign + (j ? i.substr(0, j) + thouSep : "")
                + i.substr(j).replace(/(\decSep{3})(?=\decSep)/g, "$1" + thouSep)
                + (decPlaces ? decSep + Math.abs(number - i).toFixed(decPlaces).slice(2) : "");
        },
        formatDate: function (data, showTime, showSeconds) {
            
            if (showTime === undefined) {
                showTime = true;
            }
            if (data) {
                var result = data.substring(8, 10) + '/' + data.substring(5, 7) + '/' + data.substring(0, 4) + data.substring(13,17);

                if (showTime) {
                    result += ' ' + data.substring(11, showSeconds ? 19 : 16);
                }
                return result;
            }
        },
        formatTime: function (data, showSeconds) {
            if (data) {
                return data.substring(11, showSeconds ? 19 : 16);
            }
        },
        ajaxComplete: [],
        ajaxDone: function () {
            $.each(kvs.ajaxComplete, function () {
                var fn = this;
                if (typeof fn == "function") {
                    fn();
                }
            });
        },
        initNumericTextBox: function () {
            if ($().autoNumeric) {
                $('.intText:not(.num-done)').autoNumeric('init', { digitGroupSeparator: "", decimalPlacesOverride: 0, minimumValue: -9999999999999, maximumValue: 9999999999999 }).addClass("num-done");
                $('.floatText:not(.num-done)').each(function (i, e) {
                    var mDec = 2;
                    if ($(e).attr("data-mdec")) {
                        mDec = parseInt($(e).attr("data-mdec"));
                    }

                    var aSep = ".";
                    if ($(e).attr("data-asep")) {
                        aSep = $(e).attr("data-asep");
                    }

                    $(this).autoNumeric('init', { digitGroupSeparator: aSep, decimalCharacter: aSep == "." ? "," : ".", decimalPlacesOverride: mDec }).addClass("num-done");
                });
            }
        },
        initDatePickers: function () {
            if (!$().datetimepicker) {
                return;
            }
            var options = {
                locale: "tr",
                format: "DD/MM/YYYY",
                icons: {
                    time: "fas fa-watch",
                    date: "fas fa-calendar",
                    up: "fas fa-chevron-up",
                    down: "fas fa-chevron-down",
                    previous: "fas fa-chevron-left",
                    next: "fas fa-chevron-right",
                    today: "fas fa-calendar-day"
                },
                showTodayButton: true,
                allowInputToggle: true,
                useCurrent: false,
                widgetPositioning: {
                    horizontal: 'auto',
                    vertical: 'bottom'
                },
                keyBinds: {
                    left: function (widget) {
                    },
                    right: function (widget) { }
                }
            };
            $('.date-picker:not(.date-done)').datetimepicker(options).addClass("date-done");

            options.format = "DD/MM/YYYY HH:mm";
            $('.datetime-picker:not(.date-done)').datetimepicker(options).addClass("date-done");

            options.format = "HH:mm";
            $('.time-picker:not(.date-done)').datetimepicker(options).addClass("date-done");
        },
        callFunctionByName: function (functionName, context) {
            var args = Array.prototype.slice.call(arguments, 2);
            var namespaces = functionName.split(".");
            var func = namespaces.pop();
            if (context == undefined) {
                context = window;
            }
            for (var i = 0; i < namespaces.length; i++) {
                context = context[namespaces[i]];
            }
            if (context[func] && typeof context[func] === "function") {
                return context[func].apply(context, args);
            }
        },
        grid: function (el) {
            var grd = $(el).data("grid");
            if (grd) {
                return grd;
            }
            return new KVSGrid(el);
        },
        grdEdit: function (field, data, td) {
            return $("<div>", { class: "list-icons" }).append(
                $("<div>", { class: "dropdown" }).append(
                    $("<a>", { href: "javascript:;", class: "list-icons-item dropdown-toggle caret-0", "data-toggle": "dropdown" }).append(
                        $("<i>", { class: "fas fa-bars" })
                    ),
                    $("<div>", { class: "dropdown-menu" }).append(
                        $($("<a>", { href: "javascript:;", class: "dropdown-item grid-edit" })).append(
                            $("<i>", { class: "fas fa-pencil-alt" }),
                            " Düzenle"
                        ),
                        $("<div>", { class: "dropdown-divider" }),
                        $($("<a>", { href: "javascript:;", class: "dropdown-item grid-delete text-danger" })).append(
                            $("<i>", { class: "fas fa-trash-alt" }),
                            " Sil"
                        ),
                    )
                )
            );
        },
        search: function (el) {
            var srch = $(el).data("search");
            if (srch) {
                return srch;
            }
            return new KVSSearch(el);
        }

    };
}();

var KVSGrid = function (el) {
    if ($(el).data("grid")) {
        return $(el).data("grid");
    }

    var element = $(el), $this = this, prevJx, fields = [], events = [];
    var columns = element.find("thead > tr > th");
    var body = element.find("tbody");

    var gridFooter, pagination, info, topOffset;
    var readUrl = element.attr("data-url");

    this.load = function (reset) {
        if (prevJx) {
            prevJx.abort();
        }

        var req = {
            Page: 1,
            PageSize: 20,
            Sorting: null,
            Filters: [],
            SessionKey: element.attr("data-grid-id"),
            Fields: fields
        };

        if (reset) {
            element.attr("data-page", "1");
            topOffset = 0;
        }

        if (element.find("thead > tr > th.sorting_asc").length) {
            req.Sorting = element.find("thead > tr > th.sorting_asc").attr("data-field") + ":ASC";
        }
        else if (element.find("thead > tr > th.sorting_desc").length) {
            req.Sorting = element.find("thead > tr > th.sorting_desc:first").attr("data-field") + ":DESC";
        }

        if (element.attr("data-page-size")) {
            req.PageSize = parseInt(element.attr("data-page-size"));
        }
        if (element.attr("data-page")) {
            req.Page = parseInt(element.attr("data-page"));
        }

        req.Filters = $this.getFilters();
        if (!reset) {
            topOffset = $this.scrollTop();
        }

        prevJx = kvs.callJx(readUrl, element, req, function (result) {
            if (!result || typeof result !== "object") {
                //kvs.showModal("danger", "Hata", "Veriler yüklenemedi. Lütfen daha sonra tekrar deneyiniz.");
                return;
            }

            callEvent("preRender");

            setPage(result);

            body.empty();

            if (result.total === 0) {
                callEvent("nodataLoad");
                body.append($("<tr>").append($("<td>", { colspan: columns.length, text: "Kayıt bulunamadı" })));
                return;
            }

            $.each(result.data, function (i, e) {
                var row = $("<tr>");
                $.each(columns, function (j, col) {
                    var column = $(col);
                    var td = $("<td>");
                    if (column.attr("data-template")) {
                        callTemplate(column.attr("data-field"), e, td, column.attr("data-template"));
                    }
                    else if (column.attr("data-field")) {
                        td.append(kvs.value(e, column.attr("data-field")));
                    }
                    if (column.attr("data-hidden") == "1") {
                        td.addClass("d-none");
                    }
                    row.append(td);
                });
                body.append(row);
                row.data("raw", e);
            });

            if (topOffset > 0) {
                $this.scrollTop(topOffset);
            }

            callEvent("load");
        });
    }

    this.reload = function () {
        $this.load(true);
    }

    this.getFilters = function () {
        var filters = [];
        if (element.attr("data-filter")) {
            var filter = element.attr("data-filter");
            $(":input[data-field]", filter).each(function (i, e) {
                var item = $(e);
                var value = item.val();
                if (value) {
                    filters.push({
                        Field: item.attr("data-field"),
                        Operant: item.attr("data-operant") ? item.attr("data-operant") : "*",
                        Value: value
                    });
                }
            });
        }
        return filters;
    }

    this.download = function (format) {
        if (!$("#excelForm").length) {
            $("body").append('<form id="excelForm" method="post" action=""></form>');
        }

        if (kvs.isEmpty(format)) {
            format = "excel";
        }
        $("#excelForm").empty();

        var filters = $this.getFilters();

        if (filters) {
            $.each(filters, function (i, e) {
                $("#excelForm").append($("<input>", { type: "hidden", name: "Filters[" + i + "].Field", value: e.Field }));
                $("#excelForm").append($("<input>", { type: "hidden", name: "Filters[" + i + "].Operant", value: e.Operant }));
                $("#excelForm").append($("<input>", { type: "hidden", name: "Filters[" + i + "].Value", value: e.Value }));
            });
        }

        if (element.find("thead > tr > th.sorting_asc").length) {
            $("#excelForm").append($("<input>", { type: "hidden", name: "Sorting", value: (element.find("thead > tr > th.sorting_asc").attr("data-field") + ":ASC") }));
        }
        else if (element.find("thead > tr > th.sorting_desc").length) {
            $("#excelForm").append($("<input>", { type: "hidden", name: "Sorting", value: (element.find("thead > tr > th.sorting_desc:first").attr("data-field") + ":DESC") }));
        }

        $("#excelForm").append($("<input>", { type: "hidden", name: "Page", value: "1" }));
        $("#excelForm").append($("<input>", { type: "hidden", name: "PageSize", value: "0" }));
        $("#excelForm").append($("<input>", { type: "hidden", name: "format", value: format }));

        $("#excelForm").attr('action', readUrl);
        $("#excelForm").submit();
    };

    this.getElement = function () {
        return element;
    }

    this.id = function () {
        return element.attr("id");
    }

    this.on = function (name, fn) {
        if (!name || name == "") {
            return;
        }
        if (typeof fn != "function") {
            return;
        }
        events.push({ name: name, fn: fn });
    }

    this.readUrl = function (newUrl) {
        if (newUrl) {
            readUrl = newUrl;
        }

        return readUrl;
    }

    var callEvent = function (name) {
        if (events) {
            for (var i = 0; i < events.length; i++) {
                if (events[i].name == name) {
                    events[i].fn.call($this);
                }
            }
        }
    }

    this.scrollTop = function (top) {
        if (top === undefined) {
            return element.parents(".table-responsive:first").scrollTop();
        }
        setTimeout(function () {
            element.parents(".table-responsive:first").scrollTop(top);
        }, 100);
    }

    var setPage = function (result) {

        var page = result.hasError ? 1 : result.currentPage;
        var totalPages = result.hasError ? 1 : result.totalPage;
        var total = result.hasError ? 0 : result.total;
        var adjacents = 3;

        pagination.empty();
        pagination.append(
            $("<li>", { class: "page-item " + (page > 1 ? "" : " disabled") }).append(
                $("<a>", { class: "page-link", "data-page": (page - 1), href: "javascript:;", html: "<span>«&nbsp;Geri</span>" })
            )
        );
        if (totalPages > 1) {
            if (totalPages < 7 + (adjacents * 2)) { //not enough pages to bother breaking it up
                for (var i = 1; i <= totalPages; i++) {
                    pagination.append(
                        $("<li>", { class: "page-item" + (i == page ? " active" : "") }).append(
                            $("<a>", { class: "page-link", "data-page": i, href: "javascript:;", html: i })
                        )
                    );
                }
            }
            else if (totalPages > 5 + (adjacents * 2)) { //enough pages to hide some
                if (page < 1 + (adjacents * 2)) { //close to beginning; only hide later pages
                    for (var i = 1; i < 4 + (adjacents * 2); i++) {
                        pagination.append(
                            $("<li>", { class: "page-item" + (i == page ? " active" : "") }).append(
                                $("<a>", { class: "page-link", "data-page": i, href: "javascript:;", html: i })
                            )
                        );
                    }
                    pagination.append(
                        $("<li>", { class: "page-item" }).append(
                            $("<a>", { class: "page-link", href: "javascript:;", html: "..." })
                        )
                    );
                    pagination.append(
                        $("<li>", { class: "page-item" }).append(
                            $("<a>", { class: "page-link", "data-page": totalPages, href: "javascript:;", html: totalPages })
                        )
                    );
                }
                else if (totalPages - (adjacents * 2) > page && page > (adjacents * 2)) { //in middle; hide some front and some back
                    pagination.append(
                        $("<li>", { class: "page-item" }).append(
                            $("<a>", { class: "page-link", "data-page": "1", href: "javascript:;", html: "1" })
                        )
                    );

                    pagination.append(
                        $("<li>", { class: "page-item" }).append(
                            $("<a>", { class: "page-link", href: "javascript:;", html: "..." })
                        )
                    );

                    for (var i = page - adjacents; i <= page + adjacents; i++) {
                        pagination.append(
                            $("<li>", { class: "page-item" + (i == page ? " active" : "") }).append(
                                $("<a>", { class: "page-link", "data-page": i, href: "javascript:;", html: i })
                            )
                        );
                    }

                    pagination.append(
                        $("<li>", { class: "page-item" }).append(
                            $("<a>", { class: "page-link", href: "javascript:;", html: "..." })
                        )
                    );
                    pagination.append(
                        $("<li>", { class: "page-item" }).append(
                            $("<a>", { class: "page-link", "data-page": totalPages, href: "javascript:;", html: totalPages })
                        )
                    );
                }
                else { //close to end; only hide early pages
                    pagination.append(
                        $("<li>", { class: "page-item" }).append(
                            $("<a>", { class: "page-link", "data-page": "1", href: "javascript:;", html: "1" })
                        )
                    );

                    pagination.append(
                        $("<li>", { class: "page-item" }).append(
                            $("<a>", { class: "page-link", href: "javascript:;", html: "..." })
                        )
                    );

                    for (var i = totalPages - (2 + (adjacents * 2)); i <= totalPages; i++) {
                        pagination.append(
                            $("<li>", { class: "page-item" + (i == page ? " active" : "") }).append(
                                $("<a>", { class: "page-link", "data-page": i, href: "javascript:;", html: i })
                            )
                        );
                    }
                }
            }
        }
        else {
            pagination.append(
                $("<li>", { class: "page-item disabled" }).append(
                    $("<a>", { class: "page-link", href: "javascript:;", html: "1" })
                )
            );
        }

        pagination.append(
            $("<li>", { class: "page-item " + (page < totalPages ? "" : " disabled") }).append(
                $("<a>", { class: "page-link", "data-page": (page + 1), href: "javascript:;", html: "<span>İleri&nbsp;»</span>" })
            )
        );

        if (!info.find(".total-count").length) {
            info.append(
                $("<div>", { class: "d-flex flex-row" }).append(
                    $("<div>", { class: "mt-2 mr-1", html: kvs.format("Toplam {0} kayıt", '<strong class="total-count"></strong>') }),
                    $("<div>", { style: "width: 5rem" }).append(
                        $("<select>", { class: "form-control" }).append(
                            '<option value="20">20</option>',
                            '<option value="50">50</option>',
                            '<option value="100">100</option>',
                            '<option value="1000">1000</option>'
                        )
                    )
                )
            );
            info.find("select").change(function () {
                element.attr("data-page", "1");
                element.attr("data-page-size", $(this).val());
                $this.load();
            }).val(element.attr("data-page-size"));
        }

        info.find(".total-count").html(total);
    }

    var clearColumnSorting = function () {
        $.each(columns, function (i, e) {
            var column = $(e);
            if (column.hasClass("sorting_desc") || column.hasClass("sorting_asc") || column.hasClass("sorting")) {
                column.removeClass("sorting_asc").removeClass("sorting_desc").addClass("sorting");
            }
        });
    }

    var callTemplate = function (field, data, td, template) {
        if (template.indexOf(".") >= 0) {
            var temps = template.split(".");
            var temp = window[temps[0]];
            for (var i = 1; i < temps.length; i++) {
                if (temp && temp[temps[i]]) {
                    temp = temp[temps[i]];
                }
                else {
                    break;
                }
            }

            if (temp && typeof temp === "function") {
                var result = temp.call(this, field, data, td);
                if (result) {
                    td.append(result);
                }
            }
            else {
                td.append(kvs.value(data, field));
            }
        }
        else {
            if (window[template] && typeof window[template] === "function") {
                var result = window[template].call(this, field, data, td);
                if (result) {
                    td.append(result);
                }
            }
            else {
                td.append(kvs.value(data, field));
            }
        }
    }

    element.data("grid", this);

    gridFooter = $("<div>", { class: "d-flex p-1" });
    gridFooter.insertAfter(element.parent());
    pagination = $("<div>", { class: "pagination pagination-rounded flex-grow-1 mt-2" })
    pagination.appendTo(gridFooter);
    info = $("<div>", { class: "p-1 pt-2" });
    info.appendTo(gridFooter);

    body.append($("<tr>").append($("<td>", { colspan: columns.length, text: "Kayıtlar yüklenmedi" })));
    columns.each(function () {
        var column = $(this);
        if (column.attr("data-orderable") == "1") {
            column.addClass("sorting");
        }
        if (!kvs.isEmpty(column.attr("data-order"))) {
            column.addClass(column.attr("data-order").toLowerCase() == "asc" ? "sorting_asc" : "sorting_desc");
        }
        if (!kvs.isEmpty(column.attr("data-field"))) {
            fields.push(column.attr("data-field"));
        }
    });

    element.on("click", ".sorting,.sorting_desc,.sorting_asc", function () {
        var item = $(this);
        if (item.hasClass("sorting_desc") || item.hasClass("sorting")) {
            clearColumnSorting();
            item.removeClass("sorting").removeClass("sorting_desc").addClass("sorting_asc");
        }
        else if (item.hasClass("sorting_asc")) {
            item.removeClass("sorting").removeClass("sorting_asc").addClass("sorting_desc");
        }
        $this.load();
    });

    pagination.on("click", "[data-page]", function () {
        var item = $(this);
        element.attr("data-page", item.attr("data-page"));
        $this.load();
    });

    if (element.attr("data-filter")) {
        var filter = element.attr("data-filter");
        $(filter).find("select, .date-picker").on("change", function () {
            $this.reload();
        });
        $(filter).find("input").on("keyup", function (e) {
            if (e.keyCode == 13) {
                $this.reload();
            }
        });
        $(filter).find(".search-btn,:input[type='submit']").on("click", function (e) {
            $this.reload();
        });
    }

    element.on("click", ".grid-edit", function () {
        var row = $(this).parents("tr:first");
        var data = row.data("raw");
        if (window["editRecord"] && typeof window["editRecord"] == "function") {
            window["editRecord"].call($this, data);
        }
    });

    element.on("click", ".grid-delete", function () {
        var row = $(this).parents("tr:first");
        var data = row.data("raw");
        if (window["deleteRecord"] && typeof window["deleteRecord"] == "function") {
            window["deleteRecord"].call($this, data);
        }
    });

    var loadData = true;
    if (element.is("[data-prevent-load]")) {
        loadData = element.attr("data-prevent-load") != "1";
    }
    if (loadData) {
        $this.load();
    }
}

var KVSSearch = function (el) {

    if ($(el).find(":input[data-id='1']").data("search")) {
        return $(el).find(":input[data-id='1']").data("search");
    }

    var element = $(el).find(":input[data-id='1']"), elementDisplay = $(el).find(":input[data-display='1']"), popup;
    var $this = this, prevJx, prevSearch, fields = [], events = [];
    var configDiv = element.parent().next();
    var columns = configDiv.find("[data-columns] > div[data-col-type='1']");
    var displayColumn = configDiv.find("[data-columns] > div[data-display='1']:first");
    if (!displayColumn.length) {
        displayColumn = configDiv.find("[data-columns] > div[data-col-type='1']:first");
    }
    var searchFields = [];
    configDiv.find("[data-columns] > div[data-search-in='1']").each(function () {
        searchFields.push($(this).attr("data-field"));
    });
    if (!searchFields.length) {
        configDiv.find("[data-columns] > div[data-col-type='1']").each(function () {
            searchFields.push($(this).attr("data-field"));
        });
    }
    var idField = configDiv.find("[data-columns] > div[data-col-type='0']:first").attr("data-field");
    var clearIfEmpy = element.attr("data-clear-if-empty") == "1";
    var emptyValue = element.attr("data-empty-value");
    var startSearch = parseInt(element.attr("data-start-search"));
    var newText = $("<div>", { class: "d-none text-success", html: "Yeni Kayıt", style: "position:absolute;right:0;margin:0.5rem 3rem 0 0;z-index:10000;" }).appendTo(element.parents(".input-group:first"));
    var inputTimeout, currentIndex = -1, dataLength = 0, selectedData;
    var readUrl = element.attr("data-url");

    this.load = function () {
        if (prevJx) {
            prevJx.abort();
        }
        var req = {
            Page: 1,
            PageSize: 20,
            Sorting: null,
            Filters: $this.getFilters(false),
            SessionKey: element.attr("data-grid-id"),
            Fields: fields
        };

        if (element.attr("data-page-size")) {
            req.PageSize = parseInt(element.attr("data-page-size"));
        }

        var table = popup.find("table");
        table.empty();
        table.append(
            $("<tr>").append(
                $("<td>", { colspan: columns.length, class: "p-2", text: "Yükleniyor..." })
            )
        );

        currentIndex = -1;
        dataLength = 0;
        prevJx = kvs.callJx(readUrl, ".dummy", req, function (result) {
            if (!result || typeof result !== "object") {
                kvs.showModal("danger", "Hata", "Veriler yüklenemedi. Lütfen daha sonra tekrar deneyiniz");
                return;
            }

            if (!elementDisplay.is(":focus")) {
                elementDisplay.trigger("blur");
                return;
            }

            callEvent("preRender");

            table.empty();

            dataLength = result.total;
            if (result.total === 0) {
                table.append($("<tr>").append($("<td>", { colspan: columns.length, text: "Kayıt bulunamadı" })));
                if (!clearIfEmpy) {
                    newText.removeClass("d-none");
                }

                return;
            }

            $.each(result.data, function (i, e) {
                addRow(e, table);
            });

            if (!clearIfEmpy) {
                newText.addClass("d-none");
            }

            callEvent("load");

        });
    }

    this.getFilters = function (byId) {
        var filters = [];
        filters.push({
            Field: byId ? idField : searchFields.join(","),
            Operant: byId ? "=" : "*",
            Value: byId ? element.val() : elementDisplay.val()
        });
        return filters;
    }

    this.value = function (data, notrigger) {
        if (data === undefined) {
            return selectedData;
        }
        selectedData = data;
        if (selectedData == null) {
            element.val(emptyValue);
            elementDisplay.val("");
            if (!notrigger) {
                elementDisplay.trigger("change");
                elementDisplay.trigger("blur");
            }
            prevSearch = "";
        }
        else {
            element.val(kvs.value(selectedData, idField));
            elementDisplay.val(getDisplayText(selectedData));
            if (!notrigger) {
                elementDisplay.trigger("change");
                elementDisplay.trigger("blur");
            }
            prevSearch = elementDisplay.val();
        }
        newText.addClass("d-none");
        popup.hide();
    }

    this.idValue = function (id) {
        if (id === undefined) {
            return kvs.value(selectedData, idField);
        }

        var req = {
            Page: 1,
            PageSize: 1,
            Sorting: null,
            Filters: $this.getFilters(true),
            SessionKey: element.attr("data-grid-id"),
            Fields: fields
        };

        kvs.callJx(readUrl, elementDisplay.parent(), req, function (result) {
            if (!result || typeof result !== "object") {
                kvs.showModal("danger", "Hata", "Veriler yüklenemedi. Lütfen daha sonra tekrar deneyiniz");
                return;
            }

            if (result.total === 0) {
                dataLength = 0;
                currentIndex = -1;
                return;
            }
            dataLength = 1;
            currentIndex = -1;

            $this.value(result.data[0]);

            var table = popup.find("table");
            table.empty();
            addRow(result.data[0], table);

        });
    }

    this.on = function (name, fn) {
        if (!name || name == "") {
            return;
        }
        if (typeof fn != "function") {
            return;
        }
        events.push({ name: name, fn: fn });
    }

    this.readUrl = function (newUrl) {
        if (newUrl) {
            readUrl = newUrl;
        }

        return readUrl;
    }

    this.disabled = function (prm) {
        if (prm === undefined) {
            return $(elementDisplay).attr("disabled");
        }

        if (prm == true) {
            $(elementDisplay).attr("disabled", "true")
            $(elementDisplay).parent().find("button").attr("disabled", "true");
        }
        else {
            $(elementDisplay).removeAttr("disabled");
            $(elementDisplay).parent().find("button").removeAttr("disabled");
        }
    }

    var getDisplayText = function (data) {
        if (displayColumn.attr("data-template")) {
            callTemplate(displayColumn.attr("data-field"), data, displayColumn, displayColumn.attr("data-template"));
            return displayColumn.text();
        }
        else {
            return kvs.value(data, displayColumn.attr("data-field"));
        }
    }

    var callEvent = function (name) {
        if (events) {
            for (var i = 0; i < events.length; i++) {
                if (events[i].name == name) {
                    events[i].fn.call($this);
                }
            }
        }
    }

    var callTemplate = function (field, data, td, template) {
        if (template.indexOf(".") >= 0) {
            var temps = template.split(".");
            var temp = window[temps[0]];
            for (var i = 1; i < temps.length; i++) {
                if (temp && temp[temps[i]]) {
                    temp = temp[temps[i]];
                }
                else {
                    break;
                }
            }

            if (temp && typeof temp === "function") {
                var result = temp.call(this, field, data, td);
                if (result) {
                    td.append(result);
                }
            }
            else {
                td.append(kvs.value(data, field));
            }
        }
        else {
            if (window[template] && typeof window[template] === "function") {
                var result = window[template].call(this, field, data, td);
                if (result) {
                    td.append(result);
                }
            }
            else {
                td.append(kvs.value(data, field));
            }
        }
    }

    var dataContains = function (data, search) {
        for (var i = 0; i < searchFields.length; i++) {
            var fieldVal = kvs.value(data, searchFields[i]);
            if (!kvs.isEmpty(fieldVal) && search.toLowerCase() == fieldVal.toLowerCase()) {
                return true;
            }
        }
        return false;
    }

    var addRow = function (data, table) {
        var row = $("<tr>");
        $.each(columns, function (j, col) {
            var column = $(col);
            var td = $("<td>", { class: "p-2" });
            if (column.attr("data-template")) {
                callTemplate(column.attr("data-field"), data, td, column.attr("data-template"));
            }
            else if (column.attr("data-field")) {
                td.append(kvs.value(data, column.attr("data-field")));
            }
            row.append(td);
        });
        table.append(row);
        row.data("raw", data);
    }

    element.data("search", this);

    configDiv.find("[data-columns] > div[data-col-type]").each(function () {
        var item = $(this);
        if (item.attr("data-col-type") != "2") {
            fields.push(item.attr("data-field"));
        }
    });
    popup = $("<div>", { class: "card shadow search-table", style: "position:absolute;z-index:999;width:100%;display:none;top:" + (elementDisplay.height() * 2) + "px" });
    popup.append($("<table>", { class: "table" }));
    element.parent().append(popup);

    elementDisplay.on("keydown", function (e) {
        if (popup.is(":hidden")) {
            if (e.keyCode == 40) {
                popup.show();
            }
            return;
        }
        if (e.keyCode == 40) {
            if (currentIndex < dataLength - 1) {
                currentIndex++;
                popup.find(".table-active").removeClass("table-active");
                popup.find("tr:eq(" + currentIndex + ")").addClass("table-active");
                elementDisplay.val(getDisplayText(popup.find("tr:eq(" + currentIndex + ")").data("raw")));
            }
            e.preventDefault();
        }
        else if (e.keyCode == 38) {
            if (currentIndex > 0) {
                currentIndex--;
                popup.find(".table-active").removeClass("table-active");
                popup.find("tr:eq(" + currentIndex + ")").addClass("table-active");
                elementDisplay.val(getDisplayText(popup.find("tr:eq(" + currentIndex + ")").data("raw")));
            }
            e.preventDefault();
        }
        else if (e.keyCode == 27) {
            elementDisplay.val(prevSearch);
            popup.hide();
        }
        else if (e.keyCode == 13) {
            var selectedItem = popup.find(".table-active");
            if (selectedItem.length) {
                $this.value(selectedItem.data("raw"));
            }
            e.preventDefault();
        }
    });
    elementDisplay.on("input keyup", function (e) {
        if (e.keyCode) {
            if (e.keyCode == 38 || e.keyCode == 13 || e.keyCode == 27 || e.keyCode == 40) {
                e.preventDefault();
                return;
            }
        }
        var search = elementDisplay.val();
        if (search.length >= startSearch) {
            if (search != prevSearch) {
                if (inputTimeout) {
                    clearTimeout(inputTimeout);
                }
                inputTimeout = setTimeout(function () {
                    popup.show();
                    prevSearch = search;
                    $this.load();
                }, 300);
            }
            else {
                popup.show();
            }
        }
        else {
            currentIndex = -1;
            dataLength = 0;
            popup.find("table").empty();
            popup.hide();
            if (!clearIfEmpy) {
                if (kvs.isEmpty(search)) {
                    newText.addClass("d-none");
                }
                else {
                    newText.removeClass("d-none");
                }
            }
        }
    });
    elementDisplay.on("blur", function (e) {
        if (prevJx) {
            prevJx.abort();
        }

        setTimeout(function () {
            var search = elementDisplay.val();
            if (kvs.isEmpty(search)) {
                if (clearIfEmpy) {
                    $this.value(null, true);
                }
                else {
                    element.val(emptyValue);
                    newText.addClass("d-none");
                }
            }
            else {
                if (!selectedData) {
                    popup.find("tr").each(function () {
                        if (selectedData) {
                            return;
                        }
                        var data = $(this).data("raw");
                        if (dataContains(data, search)) {
                            selectedData = data;
                        }
                    });
                }
                else {
                    if (!dataContains(selectedData, search)) {
                        selectedData = null;
                    }
                }

                if (!selectedData) {
                    if (clearIfEmpy) {
                        $this.value(null, true);
                    }
                    else {
                        element.val(emptyValue);
                        newText.removeClass("d-none");
                    }
                }
                else {
                    $this.value(selectedData, true);
                    newText.addClass("d-none");
                }
            }
            popup.hide();
        }, 100);
    });
    elementDisplay.on("focus", function (e) {
        if (dataLength > 0) {
            popup.show();
        }
    });

    popup.on("click", "table tr", function () {
        var raw = $(this).data("raw");
        if (raw) {
            $this.value(raw);
            popup.find(".table-active").removeClass("table-active");
            $(this).addClass("table-active");
        }
    });

    if (!kvs.isEmpty(element.val()) && element.val() != emptyValue) {
        $this.idValue(element.val());
    }

}

$().ready(function () {
    $.validator.setDefaults({
        errorClass: "invalid-feedback pl-3",
        highlight: function (element, errorClass) {
            $(element).addClass("is-invalid");
        },
        unhighlight: function (element, errorClass) {
            $(element).removeClass("is-invalid");
        },
        errorPlacement: function (error, element) {
            if ($(element).parent(".input-group").length) {
                error.insertAfter($(element).parent(".input-group"));
            }
            else {
                error.insertAfter(element);
            }
        }
    });



    jQuery.validator.addMethod("passwordLen", function (value, element) {
        var passw = /^(?=.*[a-z])(?=.*[A-Z]).{8,40}$/;
        return this.optional(element) || value.match(passw);
    }, "Şifre en az 8 karakter olmalı ve en az bir adet büyük ve kücük harf icermelidir.");

    kvs.initNumericTextBox();
    kvs.initDatePickers();
    $(".grid").each(function () {
        kvs.grid(this);
    });
    $(".search-input").each(function () {
        kvs.search(this);
    });
    kvs.ajaxComplete.push(function () {
        kvs.initNumericTextBox();
        kvs.initDatePickers();
        $(".grid").each(function () {
            kvs.grid(this);
        });
        $(".search-input").each(function () {
            kvs.search(this);
        });
    });
});