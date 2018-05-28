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
                    <slot name="headActionOptions">
                        <DropdownItem name='Refresh'>{{$t('Public.Refresh')}}</DropdownItem>
                        <DropdownItem name='Create'>{{$t('Public.Create')}}</DropdownItem>
                    </slot>
                    <slot name="appendHeadActionOptions"></slot>
                </DropdownMenu>
            </Dropdown>
            <Row v-if="showSearchFilter">
                <slot name="search" v-bind:searchData="searchData"></slot>
                <slot name="searchBtn">
                    <span style="margin: 0 10px;">
                        <Button @click="getpage" type="primary" icon="search">{{$t('Public.Search')}}</Button>
                        <slot name="appendSearchBtn"></slot>
                    </span>
                </slot>
            </Row>
            <Row class="margin-top-10 searchable-table-con1">
              <Table @on-sort-change="pageSort" :row-class-name="tableRowClassMethod" :columns="columns" border :data="tableData"></Table>
              <div style="text-align: right">
                <Page class="margin-top-10" :total="totalCount" @on-change="pageChange" @on-page-size-change="pagesizeChange":page-size="pageSize" :current="currentPage"></Page>
              </div>
            </Row>
        </Card>
        <Modal v-model="modalState.showModal" :title="modalState.title" :width="modalWidth">
            <div>
                <Form ref="modalForm" label-position="top" :rules="rule" :model="modalState.model">
                    <slot name="modalForm" v-bind:model="modalState.model"></slot>
                </Form>
            </div>
            <div slot="footer">
                <Button @click="modalState.showModal=false">{{$t('Public.Cancel')}}</Button>
                <Button @click="save" type="primary">{{$t('Public.Save')}}</Button>
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
          click: async () => {
            vm.modalState.model = await vm.getEditModel(params.row);
            vm.modalState.state = "edit";
            vm.modalState.showModal = true;
            vm.modalState.title = vm.$t('Public.Edit') + vm.$t(vm.title);
            vm.$parent.$emit('set-modalState','edit');
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
              onOk: () => {
                  vm.$parent.$emit('on-deleteRow', params.row.id,function(){
                      vm.getpage();
                  });
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
    defaultSorting:{
      type: String
    },
    rule: {
      type: Object
    },
    getCreateModel: {
      type: Function,
      default: function(){
          return {};
      }
    },
    getEditModel: {
      type: Function,
      default: function(row){
          return row;
      }
    },
    modalWidth: {
      type: [Number, String]
    },
    showSearchFilter: {
      type: Boolean,
      default: false
    },
    searchPage:{
        type: Function,
        default: function(filter){
            return {
                totalCount: 0,
                items: []
            }
        }
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
    async save() {
      var _this = this;
      this.$refs.modalForm.validate(async val => {
        if(!_this.ignorePower && _this.modalState.state === 'edit' 
            && 'tenantId' in _this.modalState.model && _this.modalState.model.tenantId != this.$store.state.session.tenantId)
        {
            this.$Modal.error({
                title: 'error',
                content: _this.$t('Public.UnPower')
            });
            return;
        }
        if (val) {
            _this.$parent.$emit(_this.modalState.state === 'create' ? 'on-createRow' : 'on-editRow',_this.modalState.model,function(){
                _this.modalState.showModal = false;
                _this.getpage();
            })
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
    pageSort(column){
      this.sorting = column.order === "asc" ? column.key : null;
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
      if(this.sorting){
        page["sorting"] = this.sorting;
      }
      let result = await this.searchPage({ params: page });
      this.state.tableData = [];
      this.state.tableData.push(...result.items);
      this.state.totalCount = result.totalCount;
    },
    async handleClickActionsDropdown(name) {
      if (name === "Create") {
        this.modalState.model = await this.getCreateModel();
        this.$refs.modalForm.resetFields();
        this.modalState.state = 'create';
        this.modalState.title = this.$t('Public.Create') + this.$t(this.title)
        this.modalState.showModal = true;
        this.$parent.$emit('set-modalState','create');
      } else if (name === "Refresh") {
        this.getpage();
      }
      else{
          this.$emit('on-ClickActionsDropdown', name);
      }
    }
  },
  data() {
    let _this = this;
    if (this.columnsetting.actionOption) {
      this.columnsetting.columns.push({
        title: this.$t('Public.Actions'),
        key: "action",
        width: 150,
        render: (h, params) => rowActionRender(h, params, this, this.columnsetting.actionOption)
      });
    }
    return {
      columns: this.columnsetting.columns,
      searchData: {},
      state: {
        tableData: [],
        totalCount: 0,
        pageSize: 10,
        currentPage: 1
      },
      modalState: {
        showModal: false,
        model: {},
        title: null,
        state: null
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
    this.sorting = this.defaultSorting;
    this.getpage();
  }
};
</script>