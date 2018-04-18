<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newProductClassRule"
        :editRule="productClassRule">
        <template slot="newform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="L('Details')" name="detail">
                    <FormItem :label="L('ClassName')" prop="className">
                        <Input v-model="slotProps.createModel.ClassName" :maxlength="200" :minlength="1"></Input>
                    </FormItem>
                    <FormItem :label="L('PTId')" prop="pTId">
                        <Input v-model="slotProps.createModel.pTId" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem :label="L('PostTaxRate')" prop="postTaxRate">
                        <Input v-model.number="slotProps.createModel.postTaxRate"></Input>
                    </FormItem>
                    <FormItem :label="L('BCTaxRate')" prop="bCTaxRate">
                        <Input v-model.number="slotProps.createModel.bCTaxRate"></Input>
                    </FormItem>
                </TabPane>
            </Tabs>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="L('Details')" name="detail">
                    <FormItem :label="L('ClassName')" prop="className">
                        <Input v-model="slotProps.editModel.ClassName" :maxlength="200" :minlength="1"></Input>
                    </FormItem>
                    <FormItem :label="L('PTId')" prop="pTId">
                        <Input v-model="slotProps.editModel.pTId" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem :label="L('PostTaxRate')" prop="postTaxRate">
                        <Input v-model.number="slotProps.editModel.postTaxRate"></Input>
                    </FormItem>
                    <FormItem :label="L('BCTaxRate')" prop="bCTaxRate">
                        <Input v-model.number="slotProps.editModel.bCTaxRate"></Input>
                    </FormItem>
                    <FormItem>
                        <Checkbox v-model="slotProps.editModel.isActive">{{'IsActive'|l}}</Checkbox>
                    </FormItem>
                </TabPane>
            </Tabs>
        </template>
    </simplePage>
</template>
<script>
import simplePage from "../../../components/simplepage.vue";
import ProductClassApi from "@/api/productClass";

export default {
  components: {
    simplePage
  },
  data() {
    return {
      title: "ProductClass",
      api: ProductClassApi,
      newProductClassRule: {
        className: [{ require: true }],
        pTId: [{ require: true }],
        postTaxRate: [{ type: "number" }],
        bCTaxRate: [{ type: "number" }]
      },
      productClassRule: {
        className: [{ require: true }],
        pTId: [{ require: true }],
        postTaxRate: [{ type: "number" }],
        bCTaxRate: [{ type: "number" }]
      },
      columnsetting: {
        needAction: true,
        columns: [
          {
            title: this.L("ClassName"),
            key: "className"
          },
          {
            title: this.L("PTId"),
            key: "pTId"
          },
          {
            title: this.L("PostTaxRate"),
            key: "postTaxRate"
          },
          {
            title: this.L("BCTaxRate"),
            key: "bCTaxRate"
          },
          {
            title: this.L("IsActive"),
            render: (h, params) => {
              return h("Checkbox", {
                props: {
                  value: params.row.isActive,
                  disabled: true
                }
              });
            }
          }
        ]
      }
    };
  }
};
</script>