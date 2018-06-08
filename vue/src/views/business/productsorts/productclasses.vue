<template>
  <div>
    <div>
        <Table :columns="columns" border :data="state.tableData"></Table>
        <Page :total="state.totalCount" @on-change="pageChange" @on-page-size-change="pagesizeChange" :page-size="state.pageSize" :current="state.currentPage"></Page>
    </div>
    <Modal v-model="modalState.showModal" :title="modalState.title">
        <div>
            <Form ref="modalForm" label-position="top" :rules="rule" :model="modalState.model">
              <FormItem :label="$t('ProductClasses.ClassName')" prop="className">
                  <Input v-model="modalState.model.className" :maxlength="200" :minlength="1"></Input>
              </FormItem>
              <FormItem :label="$t('ProductClasses.PTId')" prop="ptid">
                  <Input v-model="modalState.model.ptid" :maxlength="50"></Input>
              </FormItem>
              <FormItem :label="$t('ProductClasses.PostTaxRate')" prop="postTaxRate">
                  <InputNumber v-model.number="modalState.model.postTaxRate" style="width:100%"></InputNumber>
              </FormItem>
              <FormItem :label="$t('ProductClasses.BCTaxRate')" prop="bcTaxRate">
                  <InputNumber v-model.number="modalState.model.bcTaxRate" style="width:100%"></InputNumber>
              </FormItem>
              <FormItem v-if="modalState.actionState === 'edit'">
                  <Checkbox v-model="modalState.model.isActive">{{$t('Public.IsActive')}}</Checkbox>
              </FormItem>
            </Form>
        </div>
        <div slot="footer">
            <Button @click="modalState.showModal=false">{{$t('Public.Cancel')}}</Button>
            <Button @click="modalMethod" type="primary">{{$t('Public.Save')}}</Button>
        </div>
    </Modal>
  </div>
</template>
<script>
import ProductClassApi from "@/api/productclass";

const addHeaderRender = (h, params, vm, clickAction) => {
    return h("div",
        {
            on: {
                click: async () =>{
                    clickAction(vm);
                }
            }
        },
        [h("Icon", {
            style: {
                "font-Size": "14px",
                "padding-right":"10px"
            },
            props: {
                type:"plus"
            }
        }),h("span", params.column.title)]
    );
};
const rowActionRender = (h, params, vm) => {
    return h("div", [h("Button",
    {
        props: {
            type: "primary",
            size: "small"
        },
        style: {
            marginRight: "5px"
        },
        on: {
            click: () => {
              vm.oldPTId = params.row.ptid;
              vm.oldClassName = params.row.className;
              vm.modalState.actionState = "edit";
              vm.modalState.model = params.row;
              vm.modalState.showModal = true;
              vm.modalState.title = vm.$t('Public.Edit') + vm.$t('Menu.Pages.ProductClasses');
            }
        }
    },vm.$t('Public.Edit')),h("Button",
      {
        props: {
          type: "error",
          size: "small"
        },
        on: {
          click: async () => {
            vm.$Modal.confirm({
              title: vm.$t(''),
              content: vm.$t('Public.Delete') + vm.$t('Menu.Pages.ProductClasses'),
              okText: vm.$t('Public.Yes'),
              cancelText: vm.$t('Public.No'),
              onOk: () => {
                ProductClassApi.Delete(params.row.id).then(()=>{
                  vm.getpage();
                })
              }
            });
          }
        }
      },
      vm.$t('Public.Delete')
    )]);
};

export default {
  props: {
    productSortId: Number,
    productSortName: String
  },
  data() {
    var _this = this;
    const validatePTId = (rule, value, callback) => {
      if (!value) {
        callback(new Error("ptid is required"));
      } else {
        if(_this.modalState.actionState === "edit" && _this.oldPTId == value){
          callback();
          return;
        }
        ProductClassApi.VerifyPTId(_this.productSortId, value).then(function(rep) {
          if (rep.data.result) {
            callback();
          } else {
            callback("ptid is exit");
          }
        });
      }
    };
    const validateClassName = (rule, value, callback) => {
      if (!value) {
        callback(new Error("className is required"));
      } else {
        if(_this.modalState.actionState === "edit" && _this.oldClassName == value){
          callback();
          return;
        }
        ProductClassApi.VerifyClassName(_this.productSortId, value).then(function(rep) {
          if (rep.data.result) {
            callback();
          } else {
            callback("className is exit");
          }
        });
      }
    };
    return {
      rule: {
        className: [{ required: true, validator: validateClassName, trigger: 'ignore' }],
        ptid: [{ required: true, validator: validatePTId, trigger: 'ignore' }],
        postTaxRate: [{ type: "number", trigger: 'ignore' }],
        bcTaxRate: [{ type: "number", trigger: 'ignore' }]
      },
      columns: [
        {
          title: this.$t("ProductSorts.SortName"),
          key: "sortName",
          renderHeader: (h, params) => { 
            return addHeaderRender(h, params, _this, function(vm){
              vm.modalState.model = {
                className: null,
                ptid: null,
                postTaxRate: 0,
                bcTaxRate: 0
              };
              vm.modalState.showModal = true;
              vm.modalState.actionState = "create";
              vm.modalState.title = vm.$t('Public.Create') + vm.$t('Menu.Pages.ProductClasses');
          }); },
          render: (h)=>{
            return h('span',this.productSortName);
          }
        },
        {
          title: this.$t("ProductClasses.ClassName"),
          key: "className"
        },
        {
          title: this.$t("ProductClasses.PTId"),
          key: "ptid"
        },
        {
          title: this.$t("ProductClasses.PostTaxRate"),
          key: "postTaxRate"
        },
        {
          title: this.$t("ProductClasses.BCTaxRate"),
          key: "bcTaxRate"
        },
        {
          title: this.$t("Public.IsActive"),
          render: (h, params) => {
            return h("Checkbox", {
              props: {
                value: params.row.isActive,
                disabled: true
              }
            });
          }
        },
        {
          title: this.$t('Public.Actions'),
          key: "action",
          width: 150,
          render: (h, params) => rowActionRender(h, params, this)
        }
      ],
      state: {
          tableData: [],
          totalCount: 0,
          pageSize: 5,
          currentPage: 1
      },
      modalState: {
          model: {},
          title: null,
          showModal: false,
          actionState: null
      },
      oldPTId: null,
      oldClassName: null
    };
  },
  computed: {
      tableData() {
          return this.state.tableData;
      },
      totalCount() {
          return this.state.totalCount;
      },
      currentPage() {
          return this.state.currentPage;
      },
      pageSize() {
          return this.state.pageSize;
      }
  },
  methods: {
      pageChange(page) {
          this.state.currentPage = page;
          this.getpage();
      },
      pagesizeChange(pagesize) {
          this.state.pageSize = pagesize;
          this.getpage();
      },
      async getpage() {
          let page = {
              maxResultCount: this.state.pageSize,
              skipCount: (this.state.currentPage - 1) * this.state.pageSize,
              productSortId: this.productSortId
          };
          let rep = await ProductClassApi.Search({ params: page });
          this.state.tableData = [];
          this.state.tableData.push(...rep.data.result.items);
          this.state.totalCount = rep.data.result.totalCount;
      },
      modalMethod(){
          this.$refs.modalForm.validate(async val => {
            if (val) {
              this.modalState.model.ProductSortId = this.productSortId;
              if(this.modalState.model.id){
                  await ProductClassApi.Update(this.modalState.model);
              }
              else{
                  await ProductClassApi.Create(this.modalState.model);
              }
              this.modalState.showModal = false;
              this.getpage();
            }
          });
      }
  },
  async created() {
      this.getpage();
  }
};
</script>