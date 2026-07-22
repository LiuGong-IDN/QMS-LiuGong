(function ($) {
    var coreHelper = {
        downloadAsync: function (url) {
            $("iframe.download").remove();

            var iframe = document.createElement("iframe");
            iframe.className = "download";
            iframe.src = url;
            iframe.hidden = false;

            $("body").append(iframe);
        },
        getCheckedValues: function (wrapper) {
            var arr = [],
                $wrapper = $(wrapper);

            $.each($wrapper.find("input:checked"), function (k, v) {
                var $el = $(v),
                    value = $el.val();

                arr.push(value);
            });

            return arr;
        },
        format: function(str) {
            if (arguments.length) {
                var arr = Array.prototype.slice.call(arguments);
                arr.shift();
                var t = typeof arr[0];
                var key;
                var args = ("string" === t || "number" === t) ? arr : arr[0];

                for (key in args) {
                    str = str.replace(new RegExp("\\{" + key + "\\}", "gi"), args[key]);
                }
            }

            return str;
        },
        formatDate: function (date, format = 'yyyy-MM-dd') {
            if (date !== undefined && date !== null && date !== "") {
                return $.format.date(date, format);
            }
            return "";
        },
        setAuthentication: function (authentication) {
            currAuthentication = authentication;

            this.setAuthentication = null;
        },
        setBasePath: function (path) {
            basePath = path;

            this.setBasePath = null;
        },
        setApiBasePath: function (path) {
            apiBasePath = path;

            this.setApiBasePath = null;
        },
        getAuthentication: function () {
            return currAuthentication;
        },
        resolveApi: function (url) {
            return url.replace('~', apiBasePath);
        },
        resolve: function (url) {
            return url.replace('~', basePath);
        },
        radios: function ($wrapper, value) {
            $wrapper.find("input[type='radio']").removeAttr("checked");

            var $el = $wrapper.find("input[value='" + value + "']");
            $el.attr("checked", "checked");
            if ($el.length > 0) {
                $el[0].checked = true;
            }
        },
        checks: function ($wrapper, values) {
            $wrapper.find("input[type='checkbox']").removeAttr("checked");

            for (var i = 0; i < values.length; i++) {
                var $el = $wrapper.find("input[value='" + values[i] + "']");
                $el.attr("checked", "checked");
                if ($el.length > 0) {
                    $el[0].checked = true;
                }
            }
        },
        scrollTo: function(el) {
            var $obj = $.helper.getJqueryObject(el);
            
            $('html,body').animate({ scrollTop: $obj.offset().top - 135 }, 200, function () { });
        },
        changeNewLine: function (text, chr) {
            var replacedChar = typeof chr !== 'undefined' ? chr : '\n',
                regexp = new RegExp(replacedChar, 'g');

            if (text == null)
                text = '';
            return text.replace(regexp, '<br/>');
        },
        getJqueryObject: function (obj) {
            return (typeof obj === 'string') ? $('#' + obj) : (obj.jquery ? obj : $(obj));
        },
        toDefault: function (data, defaultValue) {
            return typeof data === 'undefined' || data === null || data === '' ? defaultValue : data;
        },
        traverse: function (data, childField, callback) {
            var stack = [data];
            var n;

            while (stack.length > 0) {
                n = stack.pop();
                callback(n);

                if (!n[childField]) {
                    continue;
                }

                for (var i = n[childField].length - 1; i >= 0; i--) {
                    stack.push(n[childField][i]);
                }
            }
        },
        clearError: function (el) {
            var $obj = $.helper.getJqueryObject(el);

            $obj.find(".field-validation-error").remove();
        },
        onElError: function (el, field, message) {
            //var validationMessageTmpl = kendo.template($("<script type='text/kendo-template' id='message'><div class='k-widget k-tooltip k-tooltip-validation k-invalid-msg field-validation-error' style='margin: 0.5em; display: block; ' data-for='#=field#' data-valmsg-for='#=field#' id='#=field#_validationMessage'><span class='k-icon k-i-warning'></span>#=message# <span class='k-icon k-i-close-circle' style='cursor: pointer' onclick='$.helper.close(this, \".k-widget\");'></span><div class='k-callout k-callout-n'></div></div></script>").html());
            var validationMessageTmpl = $("<script type='text/kendo-template' id='message'><div class='k-widget k-tooltip k-tooltip-validation k-invalid-msg field-validation-error' style='margin: 0.5em; display: block; ' data-for='#=field#' data-valmsg-for='#=field#' id='#=field#_validationMessage'><span class='k-icon k-i-warning'></span>#=message# <span class='k-icon k-i-close-circle' style='cursor: pointer' onclick='$.helper.close(this, \".k-widget\");'></span><div class='k-callout k-callout-n'></div></div></script>").html();

            el.parent().find(".k-tooltip-validation[data-for='" + field + "']").remove();
            el.after(validationMessageTmpl({ field: field, message: message }))
        },
        toHierarchy: function (data, idField, parentField, childrenField) {
            var roots = [];
            var all = {};

            data.forEach(function (item) {
                all[item[idField]] = item;
            })

            Object.keys(all).forEach(function (idField) {
                var item = all[idField];

                if (item[parentField] === null) {
                    roots.push(item);
                } else if (item[parentField] in all) {
                    var p = all[item[parentField]];

                    if (!(childrenField in p)) {
                        p[childrenField] = []
                    }

                    p[childrenField].push(item);
                }
            })

            return roots;
        },
        close: function (el, parEl) {
            $(el).closest(parEl).remove();
        },
        onError: function (el, args) {
            var $obj = $.helper.getJqueryObject(el),
                modelState = args.responseJSON.ModelState,
                //validationMessageTmpl = kendo.template($("<script type='text/kendo-template' id='message'><div class='k-widget k-tooltip k-tooltip-validation k-invalid-msg field-validation-error' style='margin: 0.5em; display: block; ' data-for='#=field#' data-valmsg-for='#=field#' id='#=field#_validationMessage'><span class='k-icon k-i-warning'></span>#=message# <span class='k-icon k-i-close-circle' style='cursor: pointer' onclick='$.helper.close(this, \".k-widget\");'></span><div class='k-callout k-callout-n'></div></div></script>").html());
            validationMessageTmpl = $("<script type='text/kendo-template' id='message'><div class='k-widget k-tooltip k-tooltip-validation k-invalid-msg field-validation-error' style='margin: 0.5em; display: block; ' data-for='#=field#' data-valmsg-for='#=field#' id='#=field#_validationMessage'><span class='k-icon k-i-warning'></span>#=message# <span class='k-icon k-i-close-circle' style='cursor: pointer' onclick='$.helper.close(this, \".k-widget\");'></span><div class='k-callout k-callout-n'></div></div></script>").html();

            coreHelper.clearError($obj);

            function showMessage(container, name, errors) {
                var wrapper = container.find('span[data-valmsg-for=' + name + '],span[data-val-msg-for=' + name + ']');

                wrapper.parent().append(validationMessageTmpl({ field: name, message: errors[0] }))
            }

            for (var key in modelState) {
                var arr = modelState[key];

                showMessage($obj, key.substr(key.indexOf('.') + 1), arr);
            }
        }
    }

    var controlHelper = {
        modal: {
            updateTitle: function (el, title) {
                var $el = coreHelper.getJqueryObject(el),
                    $modalTitle = $el.find(".modal-title");

                if ($modalTitle.length > 0) {
                    $modalTitle.text(title);
                }
            }
        },
        form: {
            initTrigger: function (wrapper) {
                var $wrapper = coreHelper.getJqueryObject(wrapper);

                $wrapper.on('click', '.trigger, .triggertreelist, .clear-trigger', function (e) {
                    alert('test');
                    var $el = $(this),
                        $form = $el.closest('form');

                    e.preventDefault();

                    if ($el.hasClass('trigger')) {
                        $.helper.grid.applyFilters($form);
                    } else if ($el.hasClass('triggertreelist')) {
                        $.helper.treeList.applyFilters($form);
                    } else if ($el.hasClass('clear-trigger')) {
                        $.helper.form.clear($form);
                    }
                });

                $wrapper.on('click', '.filter-trigger', function (e) {
                    var $el = $(this),
                        $form = $el.closest('form'),
                        divFilter = $form.find('.filter-div');

                    e.preventDefault();

                    if (divFilter.is(":hidden")) {
                        divFilter.show();
                        $el.html("<i class='fa fa-angle-up'></i> Hide advanced filters");
                    }
                    else {
                        divFilter.hide();
                        $el.html("<i class='fa fa-angle-down'></i> Show advanced filters");
                    }
                });
            },
            toggle: function (form, includes) {
                var $el = coreHelper.getJqueryObject(form);
                $el.find(includes).each(function (e) {
                    $(this).toggle();
                })
            },
            show: function (form, includes) {
                var $el = coreHelper.getJqueryObject(form);
                $el.find(includes).each(function (e) {
                    $(this).show();
                })
            },
            hide: function (form, includes) {
                var $el = coreHelper.getJqueryObject(form);
                $el.find(includes).each(function (e) {
                    $(this).hide();
                })
            },
            clear: function (form, excludes) {
                excludes = typeof excludes === 'undefined' ? [] : excludes;
                var $el = coreHelper.getJqueryObject(form);
                var $validator = form.validate();
                if ($validator != null)
                    $validator.resetForm();

                $el.find('input, select, textarea').each(function (e) {
                    var obj = $(this),
                        defaultValue = obj.data("defaultValue") || "",
                        includeFilter = typeof obj.data("clearFilter") === 'undefined' ? true : obj.data("clearFilter");

                    if ($.inArray(obj[0].name, excludes) == -1 && includeFilter) {
                        if (obj[0].tagName == 'SELECT') {
                            if (obj.hasClass('select2') || obj.hasClass('select2-multiple')) {
                                obj.val([]);
                                obj.trigger('change');
                            } else {
                                obj[0].selectedIndex = 0;
                                obj.trigger('change');
                            }
                        } else {
                            if (typeof obj.data("kendoUpload") !== 'undefined') {
                                obj.closest(".k-upload").find(".k-i-close").trigger("click");
                            }
                            else if (typeof obj.data("kendoDropDownList") !== 'undefined') {
                                if (obj.hasClass('k-ext-dropdown')) {
                                    var treeview = $.helper.getJqueryObject(obj.data("treeView")).data("kendoTreeView"),
                                        data = treeview.findByText("All");

                                    treeview.select(data);
                                    treeview.trigger("select", { node: data });
                                } else {
                                    obj.data("kendoDropDownList").value("-1");
                                    obj.data("kendoDropDownList").trigger("change");
                                }
                            }
                            else if (obj.attr('type') == 'file') {
                                var root = obj.closest(".fileinput");

                                if (root.length > 0) {
                                    root.fileinput('clear');
                                    root.find("a.submit").off("click").attr("disabled", "disabled");
                                }
                                //obj.closest(".fileinput").find("a[data-dismiss='fileinput']").trigger("click");
                            }
                            else if (obj.attr('type') == 'checkbox') {
                                obj.prop("checked", false);   
                            }
                            else {
                                obj.val(defaultValue);

                                if (obj.attr('data-role') == 'tagsinput') {
                                    obj.tagsinput('removeAll');
                                }
                            }
                        }
                    }
                });
            },
            serializeObject: function (form) {
                var o = {};
                var a = form.serializeArray();

                if (a.length == 0) // not a form
                    a = form.find('input,select,textarea').serializeArray();

                $.each(a, function () {
                    if (o[this.name]) {
                        if (!o[this.name].push) {
                            o[this.name] = [o[this.name]];
                        }
                        o[this.name].push(this.value || '');
                    } else {
                        o[this.name] = this.value || '';
                    }
                });
                return o;
            },
            fill: function (form, viewData, excludes) {
                if (typeof viewData === 'undefined' || viewData == null) return;

                excludes = typeof excludes === 'undefined' ? [] : excludes;
                var $el = coreHelper.getJqueryObject(form);

                var formEls = $el.find('input, select, textarea');
                var self = this,
                    tempObj = {},
                    type = '',
                    field = '';

                $.each(formEls, function (key, obj) {
                    if ($.inArray(obj.id, excludes) == -1 || $.inArray(obj.name, excludes) == -1) {
                        tempObj = $(obj);
                        field = typeof tempObj.attr('data-field') !== 'undefined' ? tempObj.data('field') : obj.name;
                        type = obj.type;

                        switch (type) {
                            case 'radio':
                                obj.checked = (obj.value != '' && viewData[field] == obj.value);
                                break;
                            case 'hidden':
                            case 'text':
                            case 'textarea':
                                var formattedValue = coreHelper.toDefault(viewData[field], '');

                                if (tempObj.hasClass("date-picker")) {
                                    var format = tempObj.data("format") || "MM/dd/yyyy";

                                    formattedValue = $.helper.formatDate(formattedValue);
                                    //formattedValue = Kendo.toString(formattedValue, format);
                                }

                                if (typeof tempObj.data("fileinput") !== 'undefined') {
                                    if (formattedValue != '') {
                                        var root = tempObj.closest(".fileinput");

                                        tempObj.val(formattedValue);

                                        root.find("a.submit").off("click").attr("disabled", "disabled");
                                        root.removeClass("fileinput-new").addClass("fileinput-exists");
                                        root.find("input[name$='OldFilename']").val("-1");
                                        root.find("input[name$='NewFilename']").val("-1");
                                        root.find(".fileinput-filename").text(formattedValue);
                                    }
                                }
                                else if (typeof tempObj.data("kendoDropDownList") !== 'undefined') {
                                    var ddl = tempObj.data("kendoDropDownList"),
                                        input = tempObj;

                                    if (ddl.options.cascadeFrom != "") {
                                        function attach() {
                                            ddl.search(formattedValue);
                                            ddl.trigger("change");
                                            ddl.unbind("dataBound");
                                        }

                                        ddl.bind("dataBound", attach);
                                    } else {
                                        ddl.search(formattedValue);
                                        ddl.trigger("change");
                                    }
                                }
                                else {
                                    tempObj.val(formattedValue);
                                }
                                break;
                            case 'select-one':
                                tempObj.val(coreHelper.toDefault(viewData[field], ''));

                                if (tempObj.hasClass('chzn-select')) {
                                    console.log("3");
                                    tempObj.trigger('change');
                                    tempObj.trigger('liszt:updated');
                                }
                                else if (tempObj.hasClass("select2-single")) {
                                    tempObj.val(viewData[field]).trigger('change');
                                }
                                else if (tempObj.hasClass("select2local")) {
                                    tempObj.val(viewData[field]).trigger('change');
                                }
                                break;
                        }
                    }
                });

                controlHelper.grid.refreshFrom($el);
            }
        },
        listView: {
            getKendoListView: function (listView) {
                var $el = coreHelper.getJqueryObject(listView);

                return $el.data('kendoListView');
            },
            refresh: function (listView) {
                var $listView = this.getKendoListView(listView);

                $listView.dataSource.read();
                $listView.refresh();
            }
        },
        treeList: {
            getKendoTreeList: function (treeList) {
                var $el = coreHelper.getJqueryObject(treeList);

                return $el.data('kendoTreeList');
            },
            applyFilters: function (form) {
                var $form = coreHelper.getJqueryObject(form),
                    $treelists = [],
                    excludes = $form.data("excludes") || [],
                    logicalOperator = $form.data('logicalOperator') || 'and',
                    $filters = $form.find('input[data-filter],select[data-filter]'),
                    currentFilters = [];

                var treelists = $form.data('treelist');

                if (treelists instanceof Array) {
                    for (var i = 0; i < treelists.length; i++) {
                        var $el = coreHelper.getJqueryObject(treelists[i]);

                        $treelists.push($el.data('kendoTreeList'));
                    }
                }
                else {
                    var $el = coreHelper.getJqueryObject(treelists);

                    $treelists.push($el.data('kendoTreeList'));
                }

                function parseFormat(value, formatType, format) {
                    switch (formatType) {
                        case 'date':
                            return $.helper.formatDate(value);
                            //return Kendo.parseDate(value, format);
                        case 'time':
                            if (value == "" || value == null) return null;

                            var splitter = format.split(":"),
                                valueSplitter = value.split(":"),
                                temp = 0;

                            function check(cat, value) {
                                if (cat == "HH") return value * 60 * 60;
                                if (cat == "MM") return value * 60;

                                return value;
                            }

                            for (var i = 0; i < splitter.length; i++) {
                                temp += check(splitter[i], valueSplitter[i]);
                            }

                            return temp;
                        case 'string':
                        default:
                            return value;
                    }
                }

                $.each($filters, function (key, val) {
                    var $el = $(val),
                        config = $el.data('filter') || {},
                        field = config.field || $el.attr('name');

                    if (excludes.length > 0 && $.inArray(field, excludes) != -1) return;

                    var separator = config.separator || ',',
                        operator = config.operator || 'eq',
                        multiple = typeof config.multiple !== 'undefined' ? config.multiple : false,
                        formatType = config.formatType || 'string',
                        format = config.format || '',
                        defaultOperator = config.defaultOperator || 'contains',
                        fieldValue = parseFormat($el.val(), formatType, format);

                    if (fieldValue != '' && fieldValue != null) {
                        if (!multiple) {
                            currentFilters.push({
                                field: field,
                                operator: operator,
                                value: fieldValue
                            });
                        } else {
                            var arr = typeof fieldValue === 'string' ? fieldValue.split(separator) : fieldValue,
                                additionalFilters = [];

                            if (arr.length == 1) {
                                operator = defaultOperator;
                            }

                            for (var i = 0; i < arr.length; i++) {
                                additionalFilters.push({
                                    field: field,
                                    operator: operator,
                                    value: arr[i]
                                });
                            }

                            currentFilters.push({
                                logic: 'or',
                                filters: additionalFilters
                            });
                        }
                    }
                });

                for (var i = 0; i < $treelists.length; i++) {
                    $treelists[i].dataSource.filter({
                        logic: logicalOperator,
                        filters: currentFilters
                    });
                }
            },
            refresh: function (treeList) {
                var $treeList = this.getKendoTreeList(treeList);

                $treeList.dataSource.read();
                $treeList.refresh();
            }
        },
        grid: {
            noData: function (el) {
                var $grid = this.getKendoGrid(el);

                return $grid.dataSource.data().length == 0;
            },
            onError: function (el) {
                return function (args) {
                    var grid = $.helper.grid.getKendoGrid(el),
                        modelState = args.xhr.responseJSON.ModelState,
                        //validationMessageTmpl = kendo.template($("<script type='text/kendo-template' id='message'><div class='k-widget k-tooltip k-tooltip-validation k-invalid-msg field-validation-error' style='margin: 0.5em; display: block; ' data-for='#=field#' data-valmsg-for='#=field#' id='#=field#_validationMessage'><span class='k-icon k-i-warning'></span>#=message#<div class='k-callout k-callout-n'></div></div></script>").html());
                    validationMessageTmpl = $("<script type='text/kendo-template' id='message'><div class='k-widget k-tooltip k-tooltip-validation k-invalid-msg field-validation-error' style='margin: 0.5em; display: block; ' data-for='#=field#' data-valmsg-for='#=field#' id='#=field#_validationMessage'><span class='k-icon k-i-warning'></span>#=message#<div class='k-callout k-callout-n'></div></div></script>").html();

                    function showMessage(container, name, errors) {
                        container.find('[data-valmsg-for=' + name + '],[data-val-msg-for=' + name + ']')
                            .replaceWith(validationMessageTmpl({ field: name, message: errors[0] }))
                    }

                    for (var key in modelState) {
                        var arr = modelState[key];

                        showMessage(grid.editable.element, key.substr(key.indexOf('.') + 1), arr);
                    }
                }
            },
            getKendoGrid: function (grid) {
                var $el = coreHelper.getJqueryObject(grid);

                return $el.data('kendoGrid');
            },
            clearFilters: function (grid) {
                var $form = coreHelper.getJqueryObject(form),
                    $grid = this.getKendoGrid($form.data('grid'));

                controlHelper.form.clear($form);
                $grid.dataSource.filter({});
            },
            applyFilters: function (form) {
                var $form = coreHelper.getJqueryObject(form),
                    excludes = $form.data("excludes") || [],
                    logicalOperator = $form.data('logicalOperator') || 'and',
                    $filters = $form.find('input[data-filter],select[data-filter]'),
                    $filtergroups = $form.find('div[data-filter-group]'),
                    currentFilters = [];

                var $grid = coreHelper.getJqueryObject($form.data('grid'));
                //console.log($grid);
                function parseFormat(value, formatType, format) {
                    switch (formatType) {
                        case 'date':
                            return $.helper.formatDate(value);
                            //return Kendo.parseDate(value, format);
                        case 'time':
                            if (value == "" || value == null) return null;

                            var splitter = format.split(":"),
                                valueSplitter = value.split(":"),
                                temp = 0;

                                function check(cat, value) {
                                    if (cat == "HH") return value * 60 * 60;
                                    if (cat == "MM") return value * 60;

                                    return value;
                                }

                            for (var i = 0; i < splitter.length; i++) {
                                temp += check(splitter[i], valueSplitter[i]);
                            }

                            return temp;
                        case 'string':
                        default:
                            return value;
                    }
                }

                $.each($filters, function (key, val) {
                    var $el = $(val),
                        config = $el.data('filter') || {},
                        field = config.field || $el.attr('name');

                    if (excludes.length > 0 && $.inArray(field, excludes) != -1) return;

                    var separator = config.separator || ',',
                        operator = config.operator || 'eq',
                        multiple = typeof config.multiple !== 'undefined' ? config.multiple : false,
                        formatType = config.formatType || 'string',
                        format = config.format || '',
                        defaultOperator = config.defaultOperator || 'contains',
                        fieldValue = parseFormat($el.val(), formatType, format);

                    if (fieldValue != '' && fieldValue != null) {
                        if (!multiple) {
                            currentFilters.push({
                                field: field,
                                operator: operator,
                                value: fieldValue
                            });
                        } else {
                            var arr = typeof fieldValue === 'string' ? fieldValue.split(separator) : fieldValue,
                                additionalFilters = [];

                            if (arr.length == 1) {
                                operator = defaultOperator;
                            }

                            for (var i = 0; i < arr.length; i++) {
                                additionalFilters.push({
                                    field: field,
                                    operator: operator,
                                    value: arr[i]
                                });
                            }

                            currentFilters.push({
                                logic: 'or',
                                filters: additionalFilters
                            });
                        }
                    }
                });

                $.each($filtergroups, function (key, val) {
                    var $el = $(val),
                        config = $el.data('filter-group') || {},
                        field = config.field || $el.attr('name');

                    var separator = config.separator || ',',
                        operator = config.operator || 'is_equal_to',
                        multiple = typeof config.multiple !== 'undefined' ? config.multiple : false,
                        formatType = config.formatType || 'string',
                        format = config.format || '',
                        defaultOperator = config.defaultOperator || 'contains',
                        fieldValue = $el.val();

                    var datafilter = $el.find("input:checked").map(function () {
                        return this.value;
                    }).get();
                    additionalFilters = [];

                    for (var i = 0; i < datafilter.length; i++) {
                        additionalFilters.push({
                            field: field,
                            operator: operator,
                            value: datafilter[i]
                        });
                    }

                    if (datafilter.length > 0) {
                        currentFilters.push({
                            logic: 'or',
                            filters: additionalFilters
                        });
                    }
                });

                $grid.KTDatatable().setDataSourceParam('filters', currentFilters);
                $grid.KTDatatable().reload();
            },
            refresh: function (grid) {
                var $grid = this.getKendoGrid(grid);

                $grid.dataSource.read();
                $grid.refresh();

                $grid.KTDatatable().reload();
            },
            refreshFrom: function (wrapper) {
                var root = this,
                    $el = coreHelper.getJqueryObject(wrapper),
                    grids = $el.find('.k-grid');

                $.each(grids, function (key, grid) {
                    root.refresh(grid);
                });
            }
        },
        infinitescroll: {
            buildFilters: function (el) {
                var $el = $(el),
                    $form = $el.closest('.form'),
                    logicalOperator = $form.data('logicalOperator') || 'and',
                    $filters = $form.find('input[data-filter],select[data-filter]'),
                    $filtergroups = $form.find('div[data-filter-group]'),
                    currentFilters = [];

                $.each($filters, function (key, val) {
                    var $el = $(val),
                        config = $el.data('filter') || {},
                        field = config.field || $el.attr('name');

                    var separator = config.separator || ',',
                        operator = config.operator || 'eq',
                        multiple = typeof config.multiple !== 'undefined' ? config.multiple : false,
                        formatType = config.formatType || 'string',
                        format = config.format || '',
                        defaultOperator = config.defaultOperator || 'contains',
                        fieldValue = $el.val();

                    
                    if (fieldValue != '' && fieldValue != null) {
                        if (!multiple) {
                            currentFilters.push({
                                field: field,
                                operator: operator,
                                value: fieldValue
                            });
                        } else {
                            var arr = typeof fieldValue === 'string' ? fieldValue.split(separator) : fieldValue,
                                additionalFilters = [];

                            if (arr.length == 1) {
                                operator = defaultOperator;
                            }

                            for (var i = 0; i < arr.length; i++) {
                                additionalFilters.push({
                                    field: field,
                                    operator: operator,
                                    value: arr[i]
                                });
                            }

                            currentFilters.push({
                                logic: 'or',
                                filters: additionalFilters
                            });
                        }
                    }
                    
                });

                $.each($filtergroups, function (key, val) {
                    var $el = $(val),
                        config = $el.data('filter-group') || {},
                        field = config.field || $el.attr('name');

                    var separator = config.separator || ',',
                        operator = config.operator || 'is_equal_to',
                        multiple = typeof config.multiple !== 'undefined' ? config.multiple : false,
                        formatType = config.formatType || 'string',
                        format = config.format || '',
                        defaultOperator = config.defaultOperator || 'contains',
                        fieldValue = $el.val();

                    var datafilter = $el.find("input:checked").map(function () {
                        return this.value;
                    }).get();
                    additionalFilters = [];

                    if (datafilter.length == 1) {
                        operator = defaultOperator;
                    }

                    for (var i = 0; i < datafilter.length; i++) {
                        additionalFilters.push({
                            field: field,
                            operator: operator,
                            value: datafilter[i]
                        });
                    }

                    if (datafilter.length>0) {
                        currentFilters.push({
                            logic: 'or',
                            filters: additionalFilters
                        });
                    }
                });

                return currentFilters;
            }
        }

    };

    $.helper = $.extend({}, controlHelper, coreHelper);

    $(function () {
        $(".check-all").on("change", function () {
            var $el = $(this),
                $table = $el.closest("table");

            if ($el.prop('checked')) {
                $table.find('tbody tr td input[type="checkbox"]:not([disabled])').each(function () {
                    $(this).prop('checked', true);
                });
            } else {
                $table.find('tbody tr td input[type="checkbox"]:not([disabled])').each(function () {
                    $(this).prop('checked', false);
                });
            }
        });

        $(document).ajaxComplete(function (event, jqxhr, settings) {
            $('[data-toggle="tooltip"]').tooltip();
        });

        $('.form[data-filter=true] .trigger, .form[data-filter=true] .triggerinfinitescroll, .form[data-filter=true] .triggertreelist, .form[data-filter=true] .clear-trigger').on('click', function (e) {
            var $el = $(this),
                $form = $el.closest('.form');

            e.preventDefault();

            if ($el.hasClass('trigger')) {
                $.helper.grid.applyFilters($form);
            } else if ($el.hasClass('triggerinfinitescroll')) {
                $.helper.infinitescroll.applyFilters($form);
            } else if ($el.hasClass('triggertreelist')) {
                $.helper.treeList.applyFilters($form);
            } else if ($el.hasClass('clear-trigger')) {
                $.helper.form.clear($form);
            }
        });

        $('form[data-filter=true] .filter-trigger').on('click', function (e) {
            var $el = $(this),
                $form = $el.closest('form'),
                divFilter = $form.find('.filter-div');

            e.preventDefault();

            if (divFilter.is(":hidden")) {
                divFilter.show();
                $el.html("<i class='fa fa-angle-up'></i> Hide advanced filters");
            }
            else {
                divFilter.hide();
                $el.html("<i class='fa fa-angle-down'></i> Show advanced filters");
            }
        });

        $("body").on("click", ".download-async", function (e) {
            var $this = $(this),
                url = $this.attr("href");

            coreHelper.downloadAsync(url);

            e.preventDefault();
            return false;
        });

        $(".ajax-loader").each(function (key, obj) {
            var $this = $(obj),
                url = $this.data("url"),
                parameters = typeof $this.data("parameters") !== 'undefined' ? $this.data("parameters") : {};

            $this.html("");

            $this.load(url, parameters, function (d) { });
        });

        $("body").on("click", ".trigger-modal", function (e) {
            var $this = $(this),
                $modal = $.helper.getJqueryObject($this.data("modal")),
                refreshComponents = typeof $this.data("refreshComponents") === 'undefined' ? false : $this.data("refreshComponents"),
                hasLoader = $modal.find(".loader").length > 0;

            if ($this.attr("data-modal-title")) {
                $modal.find(".modal-header > .modal-title").html($this.data("modalTitle"));
            }

            if (hasLoader) {
                var $loader = $modal.find(".loader"),
                    dataUrl = $loader.data("url"),
                    parameters = typeof $this.data("parameters") !== 'undefined' ? $this.data("parameters") : $loader.data("parameters"),
                    sendParameters = typeof parameters === 'string' ? window[parameters]() : parameters;

                $loader.html('');
                $modal.modal("show");
                App.blockUI({ boxed: true, target: $loader });
                $loader.load(dataUrl, sendParameters, function () {
                    App.unblockUI($loader);
                });
            } else {
                var $form = $modal.find("form");

                if ($form.length > 0) {
                    $.helper.clearError($form);
                    $.helper.form.clear($form);
                    $modifiedByEl = $form.find("input[name='ModifiedBy']");

                    if ($modifiedByEl.length > 0) {
                        $modifiedByEl.val(currAuthentication);
                    }
                }

                $modal.modal("show");
            }

            if (refreshComponents) {
                $.helper.grid.refreshFrom($modal);
            }

            e.preventDefault();
        });

        function formatRepo(repo) {
            var markup = "<div>" + repo.text + "</div>";

            return markup;
        }

        function formatRepoSelection(repo) {
            return repo.text;
        }

        $(".select2").each(function (key, val) {
            var $this = $(val),
                url = $this.data("url"),
                dataType = $this.data("dataType") || 'json',
                delay = $this.data("delay") || 1500,
                cache = typeof $this.data("cache") === 'undefined' ? false : $this.data("cache"),
                page = $this.data("page") || 10,
                parameterFunction = $this.data("parameterFunction"),
                templateResult = $this.data("templateResult"),
                templateSelection = $this.data("templateSelection"),
                minimumInputLength = $this.data("minimumInputLength") || 10;

            $this.select2({
                width: "100%",
                ajax: {
                    url: url,
                    dataType: dataType,
                    delay: delay,
                    data: function (params) {
                        var defaultParameters = {
                            q: params.term,
                            page: params.page || page
                        };

                        if (typeof parameterFunction !== 'undefined' && typeof window[parameterFunction] === 'function') {
                            $.extend(defaultParameters, window[parameterFunction]());
                        }

                        return defaultParameters;
                    },
                    processResults: function (data, params) {
                        params.page = params.page || 1;

                        return {
                            results: data.items,
                            pagination: {
                                more: (params.page * 30) < data.total_count
                            }
                        };
                    },
                    cache: cache
                },
                escapeMarkup: function (markup) { return markup; },
                minimumInputLength: minimumInputLength,
                templateResult: function (repo) {
                    if (typeof window[templateResult] === 'function') {
                        window[templateResult](repo);
                    } else {
                        return formatRepo(repo);
                    }
                },
                templateSelection: function (repo) {
                    if (typeof window[templateSelection] === 'function') {
                        window[templateSelection](repo);
                    } else {
                        return formatRepoSelection(repo);
                    }
                }
            });
        });

        $(".select2local").each(function (key, val) {
            var $this = $(val),
                dataType = $this.data("dataType") || 'json',
                delay = $this.data("delay") || 1500,
                cache = typeof $this.data("cache") === 'undefined' ? false : $this.data("cache"),
                page = $this.data("page") || 10,
                templateResult = $this.data("templateResult"),
                templateSelection = $this.data("templateSelection"),
                minimumInputLength = $this.data("minimumInputLength") || 0;

            $this.select2({
                width: "100%",
                placeholder: "",
                escapeMarkup: function (markup) { return markup; },
                minimumInputLength: minimumInputLength,
                templateResult: function (repo) {
                    if (typeof window[templateResult] === 'function') {
                        window[templateResult](repo);
                    } else {
                        return formatRepo(repo);
                    }
                },
                templateSelection: function (repo) {
                    if (typeof window[templateSelection] === 'function') {
                        window[templateSelection](repo);
                    } else {
                        return formatRepoSelection(repo);
                    }
                }
            });
        });
    });

}(jQuery));