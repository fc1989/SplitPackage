<template>
    <div>
        <Table :ref="refs" :columns="columnsList" :data="thisTableData" border disabled-hover></Table>
    </div>
</template>

<script>
import ProductClassApi from "@/api/productclass";

const labelRender = (h,value) => {
    return h("span", value);
};
const generalRender = (h, param, vm) => {
    if(!param.row.editting){
        return labelRender(h, param.row[param.column.key]);
    }
    else{
        return h("Input", {
            props: {
                type: "text",
                value: param.row[param.column.key]
            },
            on: {
                "on-blur" (event) {
                    let key = param.column.key;
                    vm.thisTableData[param.index][key] = event.target.value;
                    event.stopPropagation();
                }
            }
        });
    }
};
const dropdownRender = (h, param, vm, labelName) =>{
    if(!param.row.editting){
        return labelRender(h, param.row[labelName]);
    }
    else{
        return h(
            "Select",
            {
                props: {
                    label: vm.thisTableData[param.index][labelName],
                    value: vm.thisTableData[param.index][param.column.key],
                    filterable: true,
                    remote: true,
                    "remote-method": vm.remoteLLMethod,
                    loading: vm.loading,
                    labelInValue: true
                },
                on: {
                    "on-change"(vl,e) {
                        let key = param.column.key;
                        vm.thisTableData[param.index][key] = vl.value;
                        vm.thisTableData[param.index][labelName] = vl.label;
                        vm.$emit("input", vm.thisTableData);
                    }
                }
            },
            vm.options.map(function(op) {
                return h("Option", {
                    props: {
                        value: op.value,
                        label: op.label,
                        key: param.index + ":" + op.value
                    }
                });
            })
        );
    }
};
const addHeaderRender = (h, param, vm) => {
    return h("div",
        {
            on: {
                click: async () => {
                    var newObj = {
                        id: 0,
                        productClassId: null,
                        productClassName: null,
                        maxNum: 0,
                        minNum: 0,
                        editting: true,
                        saving: false
                    };
                    vm.thisTableData.push(newObj);
                    vm.$emit("input", vm.thisTableData);
                    // vm.$emit("on-change", vm.handleBackdata(vm.thisTableData));
                }
            }
        },
        [h("Icon", { props: {type:"android-add"}}),
        h("strong", param.column.title)]
    );
};
const actionRender = (h, param, vm) => {
    return h("div", [
        h("Button",
            {
                props: {
                    type: param.row.editting ? "success" : "primary",
                    loading: param.row.saving,
                    size: "small"
                },
                style: {
                    margin: "0 5px"
                },
                on: {
                    click: () => {
                        if (!param.row.editting) {
                            vm.thisTableData[param.index].editting = true;
                            vm.thisTableData = JSON.parse(JSON.stringify(vm.thisTableData));
                        } else {
                            vm.thisTableData[param.index].saving = true;
                            vm.thisTableData = JSON.parse(JSON.stringify(vm.thisTableData));
                            let edittingRow = vm.thisTableData[param.index];
                            edittingRow.editting = false;
                            edittingRow.saving = false;
                            vm.thisTableData = JSON.parse(JSON.stringify(vm.thisTableData));
                            vm.$emit("input", vm.thisTableData);
                            // vm.$emit("on-change", vm.handleBackdata(vm.thisTableData), param.index);
                        }
                    }
                }
            },
            param.row.editting ? vm.$t("Public.Save"): vm.$t("Public.Edit")
        ),
        h("Poptip",
            {
                props: {
                    confirm: true,
                    title: vm.$t('SplitRules.DeleteTip'),
                    transfer: true
                },
                on: {
                    "on-ok": () => {
                        vm.thisTableData.splice(param.index, 1);
                        vm.$emit("input", vm.thisTableData);
                        // vm.$emit("on-delete", vm.handleBackdata(vm.thisTableData), param.index);
                    }
                }
            },
            [
                h("Button",
                    {
                        style: {
                            margin: "0 5px"
                        },
                        props: {
                            type: "error",
                            placement: "top",
                            size: "small"
                        }
                    },
                    vm.$t("Public.Delete")
                )
            ]
        )
    ]);
};

export default {
  name: "ruleItems",
  props: {
    refs: String,
    value: Array
  },
  data() {
    var _this = this;
    return {
      columnsList: [
        {
          title: _this.$t("SplitRules.Index"),
          type: "index",
          align: "center",
          renderHeader: (h,param) => { return addHeaderRender(h, param, _this);}
        },
        {
          title: _this.$t("Menu.Pages.ProductClasses"),
          align: "center",
          key: "productClassId",
          render: (h,param) => { return dropdownRender(h, param, _this, "productClassName");}
        },
        {
          title: _this.$t("SplitRules.MaxNum"),
          align: "center",
          key: "maxNum",
          render: (h,param) => { return generalRender(h, param, _this);}
        },
        {
          title: _this.$t("SplitRules.MinNum"),
          align: "center",
          key: "minNum",
          render: (h,param) => { return generalRender(h, param, _this);}
        },
        {
          title: _this.$t("Public.Actions"),
          align: "center",
          width: 190,
          key: "action",
          render: (h, param) => { return actionRender(h, param, _this);}
        }
      ],
      thisTableData: [],
      loading: false,
      options: []
    };
  },
  methods: {
    handleBackdata(data) {
      let clonedData = JSON.parse(JSON.stringify(data));
      clonedData.forEach(item => {
        delete item.editting;
        delete item.saving;
      });
      return clonedData;
    },
    remoteLLMethod(query) {
      let _this = this;
      if (query !== "" && query !==null) {
        _this.loading = true;
        ProductClassApi.Query(query, null).then(function(req) {
          _this.options = req.data.result;
          _this.loading = false;
        });
      } else {
        _this.options = [];
      }
    }
  },
  watch: {
    value (data) {
        this.thisTableData = data.map(function(v){
            if(v.editting === undefined){
                v.editting = false;
            }
            if(v.saving === undefined){
                v.saving = false;
            }
            return v;
        });
    }
  }
};
</script>
