<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newLogisticRule"
        :editRule="logisticRule">
        <template slot="newform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="L('Details')" name="detail">
                    <FormItem :label="L('CorporationName')" prop="corporationName">
                        <Input v-model="slotProps.createModel.corporationName" :maxlength="200" :minlength="1"></Input>
                    </FormItem>
                    <FormItem :label="L('CorporationUrl')" prop="corporationUrl">
                        <Input v-model="slotProps.createModel.corporationUrl" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem :label="L('LogisticFlag')" prop="logisticFlag">
                        <Input v-model="slotProps.createModel.logisticFlag" :maxlength="50"></Input>
                    </FormItem>
                </TabPane>
            </Tabs>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="L('Details')" name="detail">
                    <FormItem :label="L('CorporationName')" prop="corporationName">
                        <Input v-model="slotProps.editModel.corporationName" :maxlength="200" :minlength="1"></Input>
                    </FormItem>
                    <FormItem :label="L('CorporationUrl')" prop="corporationUrl">
                        <Input v-model="slotProps.editModel.corporationUrl" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem :label="L('LogisticFlag')" prop="logisticFlag">
                        <Input v-model="slotProps.editModel.logisticFlag" :maxlength="50"></Input>
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
import LogisticApi from "@/api/logistic";

export default {
  components: {
    simplePage
  },
  data() {
    return {
      title: "Logistic",
      api: LogisticApi,
      newLogisticRule: {
        corporationName: [{ require: true }],
        liCorporationUrlneCode: [{ require: true }],
        logisticFlag: [{ require: true }]
      },
      logisticRule: {
        corporationName: [{ require: true }],
        liCorporationUrlneCode: [{ require: true }],
        logisticFlag: [{ require: true }]
      },
      columnsetting: {
        needAction: true,
        columns: [
          {
            title: this.L("CorporationName"),
            key: "corporationName"
          },
          {
            title: this.L("CorporationUrl"),
            key: "corporationUrl"
          },
          {
            title: this.L("LogisticFlag"),
            key: "logisticFlag"
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