<template>
    <div>
        <Card>
            <p slot="title">{{$t(title)}}</p>
            <Dropdown slot="extra"  @on-click="handleClickActionsDropdown">
                <a href="javascript:void(0)">
                    {{$t('Public.Actions')}}
                    <Icon type="android-more-vertical"></Icon>
                </a>
                <DropdownMenu slot="list">
                    <DropdownItem name='Refresh'>{{$t('Public.Refresh')}}</DropdownItem>
                    <DropdownItem name='Create'>{{$t('Public.Create')}}</DropdownItem>
                </DropdownMenu>
            </Dropdown>
            <Row v-if="showSearchFilter">
                <slot name="search" v-bind:searchData="searchData"></slot>
                <span style="margin: 0 10px;">
                  <Button @click="getpage" type="primary" icon="search">{{$t('Public.Search')}}</Button>
                  <slot name="appendBtn"></slot>
                </span>
            </Row>
            <Row class="margin-top-10 searchable-table-con1">
              <Table :row-class-name="tableRowClassMethod" :columns="columns" border :data="tableData"></Table>
              <div style="text-align: right">
                <Page :total="totalCount" class="margin-top-10" @on-change="pageChange" @on-page-size-change="pagesizeChange" :page-size="pageSize" :current="currentPage"></Page>
              </div>
            </Row>
        </Card>
        <Modal v-model="showModal" :title="$t('Public.Create') + $t(title)" :width="modalWidth">
            <div>
                <Form ref="newForm" label-position="top" :rules="newRule" :model="createModel">
                    <slot name="newform" v-bind:createModel="createModel"></slot>
                </Form>
            </div>
            <div slot="footer">
                <Button @click="showModal=false">{{$t('Public.Cancel')}}</Button>
                <Button @click="create" type="primary">{{$t('Public.Save')}}</Button>
            </div>
        </Modal>
        <Modal v-model="showEditModal" :title="$t('Public.Edit') + $t(title)" :width="modalWidth">
            <div>
                <Form ref="editForm" label-position="top" :rules="editRule" :model="editModel">
                    <slot name="editform" v-bind:editModel="editModel"></slot>
                </Form>
            </div>
            <div slot="footer">
                <Button @click="showEditModal = false">{{$t('Public.Cancel')}}</Button>
                <Button @click="edit" type="primary">{{$t('Public.Save')}}</Button>
            </div>
        </Modal>
    </div>
</template>

<script>
const rowActionRender = (h, params, vm, actionOption) => {
  var buttonArray = [];
  if(actionOption.edit && ((typeof actionOption.edit) == "boolean" ? actionOption.edit: actionOption.edit(params.row, vm))){
    buttonArray.push(h("Button",
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
            vm.editModel = JSON.parse(JSON.stringify(params.row));
            vm.showEditModal = true;
            vm.$refs.editForm.resetFields();
          }
        }
      },
      vm.$t('Public.Edit')
    ));
  }
  if(actionOption.delete && ((typeof actionOption.delete) == "boolean" ? actionOption.delete: actionOption.delete(params.row, vm))){
    buttonArray.push(h("Button",
      {
        props: {
          type: "error",
          size: "small"
        },
        on: {
          click: async () => {
            vm.$Modal.confirm({
              title: vm.$t(''),
              content: vm.$t('Public.Delete') + vm.$t(vm.title),
              okText: vm.$t('Public.Yes'),
              cancelText: vm.$t('Public.No'),
              onOk: async () => {
                await vm.api.Delete(params.row.id);
                await vm.getpage();
              }
            });
          }
        }
      },
      vm.$t('Public.Delete')
    ));
  }
  return h("div", buttonArray);
};

export default {
  props: {
    title: {
      type: String
    },
    columnsetting: {
      type: Object
    },
    api: {
      type: Object
    },
    newRule: {
      type: Object
    },
    editRule: {
      type: Object
    },
    createFormat: {
      type: Function
    },
    modalWidth: {
      type: [Number, String]
    },
    showSearchFilter: {
      type: Boolean,
      default: false
    },
    ignorePower:{
      type: Boolean,
      default: false
    },
    tableRowClassMethod:{
      type: Function,
      default: function(){
        return '';
      }
    }
  },
  methods: {
    async create() {
      var _this = this;
      this.$refs.newForm.validate(async val => {
        if (val) {
          await _this.api.Create(_this.createModel);
          _this.showModal = false;
          await _this.getpage();
        }
      });
    },
    async edit() {
      var _this = this;
      if(!_this.ignorePower && 'tenantId' in _this.editModel && _this.editModel.tenantId != this.$store.state.session.tenantId)
      {
        this.$Modal.error({
          title: 'error',
          content: _this.$t('Public.UnPower')
        });
        return;
      }
      this.$refs.editForm.validate(async val => {
        if (val) {
          await _this.api.Update(_this.editModel);
          _this.showEditModal = false;
          await _this.getpage();
        }
      });
    },
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
        skipCount: (this.state.currentPage - 1) * this.state.pageSize
      };
      if(this.searchData){
        for(var key in this.searchData){
          page[key] = this.searchData[key];
        }
      }
      let rep = await this.api.Search({ params: page });
      this.state.tableData = [];
      this.state.tableData.push(...rep.data.result.items);
      this.state.totalCount = rep.data.result.totalCount;
    },
    handleClickActionsDropdown(name) {
      if (name === "Create") {
        if(this.createFormat){
          this.createModel = this.createFormat();
          this.$refs.newForm.resetFields();
        }
        else{
          this.createModel = {};
        }
        this.showModal = true;
        this.$refs.newForm.resetFields();
      } else if (name === "Refresh") {
        this.getpage();
      }
    }
  },
  data() {
    let _this = this;
    var cm = {};
    if (this.columnsetting.actionOption) {
      this.columnsetting.columns.push({
        title: this.$t('Public.Actions'),
        key: "action",
        width: 150,
        render: (h, params) => rowActionRender(h, params, _this,this.columnsetting.actionOption)
      });
    }
    if(this.createFormat){
      cm = this.createFormat();
    } 
    return {
      columns: this.columnsetting.columns,
      searchData: {},
      editModel: {},
      createModel: cm,
      showModal: false,
      showEditModal: false,
      state: {
        tableData: [],
        totalCount: 0,
        pageSize: 10,
        currentPage: 1
      }
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
  async created() {
    this.getpage();
  }
};
</script>